using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    public class InventoryItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        [Required]
        [MaxLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal StockQuantity { get; set; } = 0;

        [Required]
        [MaxLength(20)]
        public string Unit { get; set; } = "pcs"; // ml, gram, pcs, etc.

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumStockAlert { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
