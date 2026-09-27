using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.Models
{
    // Menambahkan Index Unik pada Email dan PhoneNumber agar tidak ada data ganda
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Email menjadi sangat penting untuk Google Auth
        [MaxLength(100)]
        public string? Email { get; set; }

        // PhoneNumber dibuat Nullable (?) karena saat login Google pertama kali, 
        // kita belum mendapatkan nomor telepon user.
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        // PasswordHash dibuat Nullable (?) karena User Google tidak memiliki password lokal.
        public string? PasswordHash { get; set; }

        // Menandakan User daftar lewat mana (Local / Google)
        [Required]
        [MaxLength(20)]
        public string AuthProvider { get; set; } = "Local";

        [MaxLength(255)]
        public string? ProfilePictureUrl { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Customer";

        public string? DefaultAddress { get; set; }
        public double? DefaultLatitude { get; set; }
        public double? DefaultLongitude { get; set; }

        public Guid? MembershipTierId { get; set; }
        public MembershipTier? TierInfo { get; set; }
        public bool IsActive { get; set; } = true;

        [MaxLength(255)]
        public string? FcmToken { get; set; }

        // Relasi satu-ke-satu (Jika User ini adalah "Toko")
        public Store? OwnedStore { get; set; }

        // Relasi satu-ke-satu (Jika User ini adalah "Driver")
        public DriverProfile? DriverData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
