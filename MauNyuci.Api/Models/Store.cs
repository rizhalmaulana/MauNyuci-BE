using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    // Menambahkan Index Unik pada Email dan PhoneNumber agar tidak ada data ganda
    [Index(nameof(StoreEmail), IsUnique = true)]
    [Index(nameof(StorePhoneNumber), IsUnique = true)]
    public class Store
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        // Penting untuk fitur "Laundry Terdekat" di Flutter
        //public double? Latitude { get; set; }
        //public double? Longitude { get; set; }
        public Point? Location { get; set; }

        [MaxLength(20)]
        public string? StorePhoneNumber { get; set; }

        // Email menjadi sangat penting untuk Google Auth
        [MaxLength(100)]
        public string? StoreEmail { get; set; }

        // Jam operasional (default: "08:00 - 20:00")
        public TimeSpan OpenTime { get; set; } = new TimeSpan(8, 0, 0);  // Default: 08:00
        public TimeSpan CloseTime { get; set; } = new TimeSpan(20, 0, 0); // Default: 20:00

        public bool IsOpen { get; set; } = true;

        // Rating rata-rata (bisa diupdate tiap ada review baru)
        public double AverageRating { get; set; } = 0;

        [MaxLength(255)]
        public string? StoreImageUrl { get; set; }

        // RELASI: Satu Toko dimiliki oleh satu User (Owner)
        [Required]
        public Guid OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public User? Owner { get; set; }

        public bool HasPickupDeliveryService { get; set; } = false; // Toggle Fitur Antar-Jemput

        [Column(TypeName = "decimal(18,2)")]
        public decimal PickupDeliveryFee { get; set; } = 0; // Biaya layanan kurir

        // Opsional: Minimal order untuk bisa antar-jemput
        public decimal MinOrderForPickup { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;

        public ICollection<StoreBankAccount> BankAccounts { get; set; } = new List<StoreBankAccount>();

        // RELASI: Satu Toko memiliki banyak Layanan
        public ICollection<StoreCatalogItem> Services { get; set; } = new List<StoreCatalogItem>();
        
        // RELASI: Satu Toko memiliki banyak Driver
        public ICollection<DriverProfile> InternalDrivers { get; set; } = new List<DriverProfile>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
