using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.StoreCatalog
{
    public class StoreCatalogCreateDto
    {
        public Guid? StoreId { get; set; }

        [Required]
        public string Category { get; set; } = "Kiloan"; // Kiloan, Satuan, Karpet, dll

        [Required]
        public string ServiceType { get; set; } = "Regular"; // Regular, Express, Kilat

        [Required]
        public string Name { get; set; } = string.Empty; // "Cuci Kering Setrika"
        
        public string? Description { get; set; }

        [Required]
        public string ImageAsset { get; set; } = string.Empty; // img_laundry_item.png

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Unit { get; set; } = "Kg"; // Kg, Pcs, Meter

        [Required]
        public string TimeEstimate { get; set; } = "3 Hari"; // 3 Hari, 5 Jam, dll
    }

    public class StoreCatalogUpdateDto
    {
        public string? Category { get; set; }
        public string? ServiceType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageAsset { get; set; }
        public decimal? Price { get; set; }
        public string? Unit { get; set; }
        public string? TimeEstimate { get; set; }
        public bool? IsAvailable { get; set; }
    }

    public class StoreCatalogResponseDto
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageAsset { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string TimeEstimate { get; set; } = string.Empty;
    }
}