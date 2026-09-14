using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.Membership
{
    public class StorePromoRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string PromoCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string DiscountType { get; set; } = "Nominal"; // "Nominal" or "Percentage"

        [Required]
        public decimal DiscountValue { get; set; }

        [MaxLength(20)]
        public string DiscountTarget { get; set; } = "All"; // "All", "Service", "Delivery"

        public decimal? MaxDiscountAmount { get; set; }

        public decimal MinOrderAmount { get; set; } = 0;

        [Required]
        public DateTime ExpiryDate { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
