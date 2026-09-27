using MauNyuci.Api.Data;
using MauNyuci.Api.Middleware;
using MauNyuci.Api.Repositories.Implementations;
using MauNyuci.Api.Repositories.Interfaces;
using MauNyuci.Api.Services.Implementations;
using MauNyuci.Api.Services.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Firebase Admin SDK Init
var firebaseCredPath = Path.Combine(AppContext.BaseDirectory, "firebase-service-account.json");
if (File.Exists(firebaseCredPath))
{
    var credential = GoogleCredential.FromFile(firebaseCredPath);
    FirebaseApp.Create(new AppOptions { Credential = credential });
}

// 1. Registrasi Database & Services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => {
            o.UseNetTopologySuite();
            o.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
        }
    ));

// Konfigurasi Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer();

// Mendaftarkan Repositories
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreBankAccountRepository, StoreBankAccountRepository>();

// Mendaftarkan Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStoreBankAccountService, StoreBankAccountService>();
builder.Services.AddScoped<IStoreCatalogRepository, StoreCatalogRepository>();
builder.Services.AddScoped<IStoreCatalogService, StoreCatalogService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IFCMService, FCMService>();
builder.Services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
builder.Services.AddScoped<IReminderJobService, ReminderJobService>();

builder.Services.AddScoped<IStorePromoService, StorePromoService>();
builder.Services.AddScoped<IStoreExpenseService, StoreExpenseService>();
builder.Services.AddScoped<IStoreStaffService, StoreStaffService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IStoreAnalyticsService, StoreAnalyticsService>();

builder.Services.AddSignalR();

// 2. Registrasi JWT Authentication (Mengajari .NET cara baca token)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrEmpty(m));
            return new BadRequestObjectResult(new
            {
                success = false,
                message = "Validasi gagal",
                errors = errors
            });
        };
    });
builder.Services.AddEndpointsApiExplorer();

// 3. Konfigurasi Swagger (Menambahkan tombol Gembok)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MauNyuci API", Version = "v1" });

    // Mendefinisikan format input Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Masukkan token JWT dengan format: Bearer {token_anda}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Memaksa Swagger untuk mengirim token di setiap request yang terkunci
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MauNyuci API v1");
        c.RoutePrefix = string.Empty; // Agar langsung terbuka di localhost:xxxx
    });
}

// 3.5. Force Culture to en-US to fix double/decimal parsing (dot vs comma)
var defaultCulture = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.UseMiddleware<MauNyuci.Api.Middleware.ExceptionHandlingMiddleware>();

// 4. Mengaktifkan Middleware (URUTAN INI SANGAT PENTING!)
app.UseAuthentication(); // Siapa kamu? (Cek KTP/Token)
app.UseAuthorization();  // Boleh masuk ruangan ini tidak? (Cek Hak Akses)

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new MauNyuci.Api.Middleware.HangfireAuthFilter() }
});

app.MapControllers();
app.MapHub<MauNyuci.Api.Hubs.OrderHub>("/orderHub");

app.Run();