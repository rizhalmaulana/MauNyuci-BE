using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MauNyuci.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPromoBanners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromoBanners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    CtaText = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CtaType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CtaValue = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TargetRoles = table.Column<string[]>(type: "text[]", nullable: false),
                    TargetTiers = table.Column<string[]>(type: "text[]", nullable: false),
                    AppType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoBanners", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PromoBanners",
                columns: new[] { "Id", "AppType", "CreatedAt", "CtaText", "CtaType", "CtaValue", "Description", "Icon", "ImageUrl", "IsActive", "SortOrder", "TargetRoles", "TargetTiers", "Title" },
                values: new object[,]
                {
                    { new Guid("b1111111-1111-1111-1111-111111111111"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Coba 1 Bulan Gratis", "whatsapp", "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.", "Masih rekap nota sampai tengah malam? Pantau omzet dan profit bersih real-time dari satu layar.", "pie_chart_outline", "https://cdn.beyondtalentservice.id/promotion-banners/laporan_keuangan.png", true, 1, new[] { "Owner" }, new[] { "Regular" }, "Laporan Keuangan Otomatis" },
                    { new Guid("b1111111-1111-1111-1111-111111111112"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Coba 1 Bulan Gratis", "whatsapp", "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.", "Punya 2-3 cabang tapi takut kas bocor? Kunci akses staf dan pantau semua transaksi dalam genggaman.", "store_outlined", "https://cdn.beyondtalentservice.id/promotion-banners/multi_outlet.png", true, 2, new[] { "Owner" }, new[] { "Regular" }, "Multi-Outlet & Kunci Kas" },
                    { new Guid("b1111111-1111-1111-1111-111111111113"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Coba 1 Bulan Gratis", "whatsapp", "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.", "Baju selesai otomatis ternotifikasi, voucher terkirim ke pelanggan lama. Orderan datang sendiri.", "notifications_active_outlined", "https://cdn.beyondtalentservice.id/promotion-banners/blast_promo.png", true, 3, new[] { "Owner" }, new[] { "Regular" }, "Auto-Reminder & Blast Promo" },
                    { new Guid("b1111111-1111-1111-1111-111111111114"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Coba 1 Bulan Gratis", "whatsapp", "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.", "Deterjen dan parfum menipis langsung diingatkan. Produksi jalan terus, pelanggan tak kecewa.", "inventory_2_outlined", "https://cdn.beyondtalentservice.id/promotion-banners/manajemen_stok.png", true, 4, new[] { "Owner" }, new[] { "Regular" }, "Stok Tak Pernah Kehabisan" },
                    { new Guid("b1111111-1111-1111-1111-111111111115"), "Store", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Coba 1 Bulan Gratis", "whatsapp", "+6281234567890|Halo CS MauNyuci, saya tertarik untuk berlangganan fitur Premium untuk toko saya.", "Mau laundry-mu muncul paling atas di aplikasi Customer? Aktifkan prioritas listing sekarang!", "verified_outlined", "https://cdn.beyondtalentservice.id/promotion-banners/rekomendasi.png", true, 5, new[] { "Owner" }, new[] { "Regular" }, "Badge Toko Pilihan" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoBanners");
        }
    }
}
