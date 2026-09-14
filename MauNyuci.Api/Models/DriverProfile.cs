using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    public class DriverProfile
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Hubungkan ke tabel User (Identitas Login)
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        // Hubungkan ke tabel Store (Tempat dia bekerja)
        [Required]
        public Guid StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        [Required]
        [MaxLength(20)]
        public string VehicleNumber { get; set; } = string.Empty; // Plat Nomor

        [MaxLength(50)]
        public string VehicleType { get; set; } = "Motorcycle";

        [MaxLength(255)]
        public string? VehicleImageUrl { get; set; } // Foto Kendaraan

        public bool IsAvailable { get; set; } = true;

        // Lokasi terakhir driver (untuk fitur tracking di Flutter)
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
