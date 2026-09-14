using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    public class StoreCatalogItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty; // Kiloan, Satuan, dll.

        [Required]
        [MaxLength(50)]
        public string ServiceType { get; set; } = "Regular"; // Regular, Express

        [Required]
        public string TimeEstimate { get; set; } = "3 Hari"; // Default 3 Hari

        public string? Description { get; set; }

        [Required]
        public string ImageAsset { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Cuci Kering Setrika

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(20)]
        public string Unit { get; set; } = "Kg"; // Kg, Pcs, dll.

        // Menghubungkan layanan ini ke Toko tertentu
        public Guid StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
