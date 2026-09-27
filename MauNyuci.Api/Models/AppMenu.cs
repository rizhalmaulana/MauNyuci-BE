using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.Models
{
    public class AppMenu
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string AppType { get; set; } = string.Empty; // "Customer", "Store", "Driver"

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Path { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Icon { get; set; }

        public int SortOrder { get; set; } = 0;

        public Guid? ParentId { get; set; }
        public AppMenu? Parent { get; set; }
        public ICollection<AppMenu>? SubMenus { get; set; }

        [MaxLength(50)]
        public string? RequiredRole { get; set; }

        public Guid? RequiredMembershipTierId { get; set; }
        public MembershipTier? RequiredMembershipTier { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
