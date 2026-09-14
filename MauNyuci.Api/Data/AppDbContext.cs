using Microsoft.EntityFrameworkCore;
using MauNyuci.Api.Models;

namespace MauNyuci.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Mendaftarkan tabel Users ke PostgreSQL
        public DbSet<User> User { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<StoreCatalogItem> StoreCatalogItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<StoreBankAccount> StoreBankAccounts { get; set; }
        public DbSet<DriverProfile> DriverProfiles { get; set; }
        public DbSet<DriverSettlement> DriverSettlements { get; set; }
        public DbSet<AppMenu> AppMenus { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StorePromo> StorePromos { get; set; }
        public DbSet<StoreExpense> StoreExpenses { get; set; }
        public DbSet<StoreStaff> StoreStaffs { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<PromoBanner> PromoBanners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            var staticDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seeding Data for AppMenu
            modelBuilder.Entity<AppMenu>().HasData(
                // Store Menus
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), AppType = "Store", Title = "Beranda", Path = "/home", Icon = "ic_home", SortOrder = 1, RequiredRole = "Store", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), AppType = "Store", Title = "Pesanan", Path = "/pesanan", Icon = "ic_riwayat", SortOrder = 2, RequiredRole = "Store", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), AppType = "Store", Title = "Layanan", Path = "/layanan", Icon = "local_laundry_service", SortOrder = 3, RequiredRole = "Store", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), AppType = "Store", Title = "Akun", Path = "/akun", Icon = "ic_akun", SortOrder = 4, RequiredRole = "Store", CreatedAt = staticDate, UpdatedAt = staticDate },
                
                // Layanan Sub-menus
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111121"), ParentId = Guid.Parse("11111111-1111-1111-1111-111111111113"), AppType = "Store", Title = "Katalog Laundry", Path = "/layanan/katalog", Icon = "local_laundry_service", SortOrder = 1, RequiredRole = "Store", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111122"), ParentId = Guid.Parse("11111111-1111-1111-1111-111111111113"), AppType = "Store", Title = "Katalog Stok", Path = "/layanan/stok", Icon = "inventory", SortOrder = 2, RequiredRole = "Owner", CreatedAt = staticDate, UpdatedAt = staticDate },

                // Akun Sub-menus
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111131"), ParentId = Guid.Parse("11111111-1111-1111-1111-111111111114"), AppType = "Store", Title = "Ubah Profil Toko", Path = "/akun/profil", Icon = "store", SortOrder = 1, RequiredRole = "Owner", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111132"), ParentId = Guid.Parse("11111111-1111-1111-1111-111111111114"), AppType = "Store", Title = "Kelola Staff", Path = "/akun/staff", Icon = "people", SortOrder = 2, RequiredRole = "Owner", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111133"), ParentId = Guid.Parse("11111111-1111-1111-1111-111111111114"), AppType = "Store", Title = "Metode Pembayaran", Path = "/akun/pembayaran", Icon = "payment", SortOrder = 3, RequiredRole = "Owner", CreatedAt = staticDate, UpdatedAt = staticDate },
                
                // Premium Store Menus
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), AppType = "Store", Title = "Analitik", Path = "/analitik", Icon = "ic_analytics", SortOrder = 5, RequiredRole = "Owner", RequiredMembershipTier = "Premium", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111116"), AppType = "Store", Title = "Promo & Voucher", Path = "/promo", Icon = "ic_discount", SortOrder = 6, RequiredRole = "Owner", RequiredMembershipTier = "Premium", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("11111111-1111-1111-1111-111111111117"), AppType = "Store", Title = "Pengeluaran", Path = "/pengeluaran", Icon = "ic_expense", SortOrder = 7, RequiredRole = "Owner", RequiredMembershipTier = "Premium", CreatedAt = staticDate, UpdatedAt = staticDate },

                // Customer Menus
                new AppMenu { Id = Guid.Parse("22222222-2222-2222-2222-222222222221"), AppType = "Customer", Title = "Beranda", Path = "/home", Icon = "ic_home", SortOrder = 1, RequiredRole = "Customer", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), AppType = "Customer", Title = "Riwayat", Path = "/history", Icon = "ic_riwayat", SortOrder = 2, RequiredRole = "Customer", CreatedAt = staticDate, UpdatedAt = staticDate },
                new AppMenu { Id = Guid.Parse("22222222-2222-2222-2222-222222222223"), AppType = "Customer", Title = "Akun", Path = "/account", Icon = "ic_akun", SortOrder = 3, RequiredRole = "Customer", CreatedAt = staticDate, UpdatedAt = staticDate }
            );

            // Seeding Data for PromoBanner
            modelBuilder.Entity<PromoBanner>().HasData(
                new PromoBanner {
                    Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                    Title = "Laporan Keuangan Otomatis",
                    Description = "Masih rekap nota sampai tengah malam? Pantau omzet dan profit bersih real-time dari satu layar.",
                    Icon = "pie_chart_outline",
                    ImageUrl = "https://cdn.beyondtalentservice.id/promotion-banners/laporan_keuangan.png",
                    CtaText = "Coba 1 Bulan Gratis",
                    CtaType = "whatsapp",
                    CtaValue = "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.",
                    SortOrder = 1,
                    IsActive = true,
                    TargetRoles = new[] { "Owner" },
                    TargetTiers = new[] { "Regular" },
                    AppType = "Store",
                    CreatedAt = staticDate
                },
                new PromoBanner {
                    Id = Guid.Parse("b1111111-1111-1111-1111-111111111112"),
                    Title = "Multi-Outlet & Kunci Kas",
                    Description = "Punya 2-3 cabang tapi takut kas bocor? Kunci akses staf dan pantau semua transaksi dalam genggaman.",
                    Icon = "store_outlined",
                    ImageUrl = "https://cdn.beyondtalentservice.id/promotion-banners/multi_outlet.png",
                    CtaText = "Coba 1 Bulan Gratis",
                    CtaType = "whatsapp",
                    CtaValue = "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.",
                    SortOrder = 2,
                    IsActive = true,
                    TargetRoles = new[] { "Owner" },
                    TargetTiers = new[] { "Regular" },
                    AppType = "Store",
                    CreatedAt = staticDate
                },
                new PromoBanner {
                    Id = Guid.Parse("b1111111-1111-1111-1111-111111111113"),
                    Title = "Auto-Reminder & Blast Promo",
                    Description = "Baju selesai otomatis ternotifikasi, voucher terkirim ke pelanggan lama. Orderan datang sendiri.",
                    Icon = "notifications_active_outlined",
                    ImageUrl = "https://cdn.beyondtalentservice.id/promotion-banners/blast_promo.png",
                    CtaText = "Coba 1 Bulan Gratis",
                    CtaType = "whatsapp",
                    CtaValue = "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.",
                    SortOrder = 3,
                    IsActive = true,
                    TargetRoles = new[] { "Owner" },
                    TargetTiers = new[] { "Regular" },
                    AppType = "Store",
                    CreatedAt = staticDate
                },
                new PromoBanner {
                    Id = Guid.Parse("b1111111-1111-1111-1111-111111111114"),
                    Title = "Stok Tak Pernah Kehabisan",
                    Description = "Deterjen dan parfum menipis langsung diingatkan. Produksi jalan terus, pelanggan tak kecewa.",
                    Icon = "inventory_2_outlined",
                    ImageUrl = "https://cdn.beyondtalentservice.id/promotion-banners/manajemen_stok.png",
                    CtaText = "Coba 1 Bulan Gratis",
                    CtaType = "whatsapp",
                    CtaValue = "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.",
                    SortOrder = 4,
                    IsActive = true,
                    TargetRoles = new[] { "Owner" },
                    TargetTiers = new[] { "Regular" },
                    AppType = "Store",
                    CreatedAt = staticDate
                },
                new PromoBanner {
                    Id = Guid.Parse("b1111111-1111-1111-1111-111111111115"),
                    Title = "Badge Toko Pilihan",
                    Description = "Mau laundry-mu muncul paling atas di aplikasi Customer? Aktifkan prioritas listing sekarang!",
                    Icon = "verified_outlined",
                    ImageUrl = "https://cdn.beyondtalentservice.id/promotion-banners/rekomendasi.png",
                    CtaText = "Coba 1 Bulan Gratis",
                    CtaType = "whatsapp",
                    CtaValue = "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.",
                    SortOrder = 5,
                    IsActive = true,
                    TargetRoles = new[] { "Owner" },
                    TargetTiers = new[] { "Regular" },
                    AppType = "Store",
                    CreatedAt = staticDate
                }
            );
        }
    }
}
