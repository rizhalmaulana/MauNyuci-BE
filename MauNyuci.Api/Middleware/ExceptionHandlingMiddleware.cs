using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MauNyuci.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                success = false,
                message = GetErrorMessage(exception),
                errors = GetErrors(exception)
            };

            context.Response.StatusCode = GetStatusCode(exception);

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            await context.Response.WriteAsync(json);
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                DbUpdateException => (int)HttpStatusCode.BadRequest,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                TimeoutException => (int)HttpStatusCode.RequestTimeout,
                OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }

        private static string GetErrorMessage(Exception exception)
        {
            return exception switch
            {
                DbUpdateException dbEx when dbEx.InnerException?.Message.Contains("duplicate key") == true =>
                    "Data sudah ada",
                DbUpdateException dbEx when dbEx.InnerException?.Message.Contains("foreign key") == true =>
                    "Data tidak valid",
                DbUpdateException _ =>
                    "Gagal menyimpan data",
                ArgumentException _ =>
                    "Data tidak valid",
                KeyNotFoundException _ =>
                    "Data tidak ditemukan",
                InvalidOperationException _ =>
                    "Operasi tidak valid",
                TimeoutException _ =>
                    "Koneksi timeout, coba lagi",
                OperationCanceledException _ =>
                    "Layanan sedang tidak tersedia",
                _ => "Terjadi kesalahan, coba lagi nanti"
            };
        }

        private static object? GetErrors(Exception exception)
        {
            if (exception is DbUpdateException dbEx && dbEx.InnerException != null)
            {
                return new { detail = dbEx.InnerException.Message };
            }
            return null;
        }
    }
}