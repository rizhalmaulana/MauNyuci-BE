using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    public class StorePromo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        [Required]
        [MaxLength(50)]
        public string PromoCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string DiscountType { get; set; } = "Nominal"; // "Nominal" or "Percentage"

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        // Batas Maksimum Potongan (khusus untuk tipe Percentage)
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxDiscountAmount { get; set; }

        // Minimal Transaksi agar voucher bisa dipakai
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinOrderAmount { get; set; } = 0;

        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(20)]
        public string DiscountTarget { get; set; } = "All"; // "All", "Service", "Delivery"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
