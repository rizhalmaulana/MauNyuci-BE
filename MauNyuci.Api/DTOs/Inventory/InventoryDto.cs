using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.Inventory
{
    public class InventoryItemCreateDto
    {
        [Required]
        public string ItemName { get; set; } = string.Empty;
        
        [Required]
        public decimal StockQuantity { get; set; }
        
        [Required]
        public string Unit { get; set; } = "pcs";
        
        public decimal MinimumStockAlert { get; set; }
    }

    public class InventoryItemUpdateDto
    {
        public string? ItemName { get; set; }
        public string? Unit { get; set; }
        public decimal? MinimumStockAlert { get; set; }
    }

    public class InventoryItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal StockQuantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal MinimumStockAlert { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class InventoryTransactionCreateDto
    {
        [Required]
        public string TransactionType { get; set; } = "Out"; // In, Out, Adjustment
        
        [Required]
        public decimal Quantity { get; set; }
        
        public string? Notes { get; set; }
    }

    public class InventoryTransactionResponseDto
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }
        public Guid PerformedById { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
