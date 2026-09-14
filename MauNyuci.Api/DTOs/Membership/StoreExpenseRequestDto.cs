using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.Membership
{
    public class StoreExpenseRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; }
    }
}
