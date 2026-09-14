using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.Models
{
    public class PromoBanner
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Icon { get; set; } = string.Empty;
        
        public string ImageUrl { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string CtaText { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string CtaType { get; set; } = string.Empty;
        
        public string CtaValue { get; set; } = string.Empty;
        
        public int SortOrder { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string[] TargetRoles { get; set; } = Array.Empty<string>();
        
        public string[] TargetTiers { get; set; } = Array.Empty<string>();
        
        [MaxLength(20)]
        public string AppType { get; set; } = "Store";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
