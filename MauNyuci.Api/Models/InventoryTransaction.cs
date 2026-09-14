using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    public class InventoryTransaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid InventoryItemId { get; set; }

        [ForeignKey("InventoryItemId")]
        public InventoryItem? InventoryItem { get; set; }

        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; } = "Out"; // In, Out, Adjustment

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [MaxLength(255)]
        public string? Notes { get; set; }

        [Required]
        public Guid PerformedById { get; set; } // The User ID (Owner or Staff) who performed this

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
