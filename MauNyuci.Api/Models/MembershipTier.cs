using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.Models
{
    public class MembershipTier
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public decimal MonthlyPrice { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigasi untuk relasi ke User (bisa banyak user di tier ini)
        public ICollection<User>? Users { get; set; }

        // Navigasi untuk menu yang membutuhkan tier ini
        public ICollection<AppMenu>? RequiredForMenus { get; set; }
    }
}
