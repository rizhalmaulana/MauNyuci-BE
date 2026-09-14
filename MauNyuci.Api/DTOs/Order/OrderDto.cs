using System.ComponentModel.DataAnnotations;
using MauNyuci.Api.Models;

namespace MauNyuci.Api.DTOs.Order
{
    // DTO untuk Flutter mengirim pesanan (Checkout)
    public class OrderCreateRequestDto
    {
        [Required]
        public Guid StoreId { get; set; }

        [Required]
        public DeliveryType DeliveryType { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        // Jika DeliveryType = Courier, ini wajib diisi
        public string? DeliveryAddress { get; set; }
        public double? DeliveryLatitude { get; set; }
        public double? DeliveryLongitude { get; set; }

        public Guid? SelectedStoreBankAccountId { get; set; }

        public string? PromoCode { get; set; }

        [Required]
        public List<OrderItemRequestDto> Items { get; set; } = new List<OrderItemRequestDto>();
    }

    // DTO untuk Kasir/Toko melakukan Order Manual (POS)
    public class OrderPosRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string GuestCustomerName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? GuestCustomerPhone { get; set; }

        [Required]
        public DeliveryType DeliveryType { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public string? DeliveryAddress { get; set; }
        public double? DeliveryLatitude { get; set; }
        public double? DeliveryLongitude { get; set; }

        public Guid? SelectedStoreBankAccountId { get; set; }

        public string? PromoCode { get; set; }

        [Required]
        public List<OrderItemRequestDto> Items { get; set; } = new List<OrderItemRequestDto>();
    }

    public class OrderItemRequestDto
    {
        [Required]
        public Guid CatalogItemId { get; set; }

        // Bisa dikirim 0 dari Flutter jika layanan kiloan dan belum ditimbang
        [Range(0, double.MaxValue, ErrorMessage = "Jumlah/Berat tidak boleh negatif")]
        public decimal Quantity { get; set; } = 0;
    }

    // DTO untuk menampilkan riwayat pesanan ke User/Toko
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public bool IsManualOrder { get; set; }
        public string? GuestCustomerName { get; set; }
        public string? GuestCustomerPhone { get; set; }
        public string? CustomerPhone { get; set; } // Consolidated phone (Guest or Registered)
        public string Status { get; set; } = string.Empty;
        public string DeliveryType { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal DeliveryFee { get; set; }
        public string? AppliedPromoCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string? PaymentReceiptUrl { get; set; }
        public string? PaymentRejectionReason { get; set; }
        
        // Data rekening bank yang dipilih oleh Customer
        public Guid? SelectedStoreBankAccountId { get; set; }
        public string? SelectedStoreBankName { get; set; }
        public string? SelectedStoreBankAccountNumber { get; set; }
        public string? SelectedStoreAccountHolderName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; }
        public bool IsLate { get; set; }
        public DateTime? PaidAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }

    public class OrderItemResponseDto
    {
        public string ItemName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public string? ItemImageUrl { get; set; }
    }

    public class OrderCancelRequestDto
    {
        public string? Reason { get; set; }
    }

    public class OrderCompleteRequestDto
    {
        [Required]
        public string FinalPaymentProvider { get; set; } = "Tunai";
    }

    public class StoreTransactionsSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; } // Hanya menghitung order yang Paid & Completed
        
        // Breakdown Status Order (Menunggu, Siap Ambil, Selesai, dll)
        public int PendingOrders { get; set; }        // Menunggu Konfirmasi
        public int WashingOrders { get; set; }        // Sedang Dicuci
        public int ReadyForPickupOrders { get; set; } // Siap Ambil / Sedang Diantar
        public int CompletedOrders { get; set; }      // Selesai
        public int CancelledOrders { get; set; }      // Dibatalkan
    }

    public class OrderHeaderItemDto
    {
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; } // Harga per jenis layanan
        public string? ItemImageUrl { get; set; }
    }

    public class OrderHeaderResponseDto
    {
        public Guid Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public bool IsManualOrder { get; set; }
        public string? GuestCustomerName { get; set; }
        public string? CustomerPhone { get; set; } // Consolidated phone (Guest or Registered)
        public string Status { get; set; } = string.Empty;
        public string DeliveryType { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        
        // Total keseluruhan dari Order
        public decimal TotalAmount { get; set; }
        public decimal TotalQuantity { get; set; } // Hasil penjumlahan berat/quantity semua item
        
        public string? AppliedPromoCode { get; set; }
        public decimal DiscountAmount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;
        public string? PaymentProvider { get; set; } // e.g. "BCA", "Tunai", null if Unpaid
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; } // Ini untuk "Estimasi Hari"
        public bool IsLate { get; set; }
        public DateTime? PaidAt { get; set; }

        // Ringkasan list item agar sangat ringan (hanya nama, qty, dan satuan)
        public List<OrderHeaderItemDto> ItemsSummary { get; set; } = new List<OrderHeaderItemDto>();
    }
}