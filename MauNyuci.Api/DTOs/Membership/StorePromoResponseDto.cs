namespace MauNyuci.Api.DTOs.Membership
{
    public class StorePromoResponseDto
    {
        public Guid Id { get; set; }
        public string PromoCode { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public string DiscountTarget { get; set; } = string.Empty;
        public decimal? MaxDiscountAmount { get; set; }
        public decimal MinOrderAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
