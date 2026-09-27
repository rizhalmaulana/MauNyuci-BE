using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    public enum PaymentMethod
    {
        PayNow,   // Bayar Langsung (Transfer/E-Wallet di awal)
        PayLater  // Bayar Nanti (COD saat ambil/antar)
    }

    public enum DeliveryType
    {
        SelfService, // Pelanggan antar dan ambil sendiri ke toko
        Courier      // Menggunakan layanan kurir antar-jemput toko
    }

    public enum OrderStatus
    {
        Pending,            // Menunggu konfirmasi toko
        Confirmed,          // Diterima oleh toko

        WaitingForDropOff,  // [Self-Service] Menunggu pelanggan mengantar cucian ke toko
        OnPickup,           // [Courier] Kurir sedang menjemput cucian pelanggan
        AwaitingPayment,    // Baju sudah ditimbang, ditahan menunggu transfer masuk
        Washing,            // Cucian sedang diproses (dicuci/disetrika)

        ReadyForPickup,     // [Self-Service] Selesai dicuci, menunggu diambil pelanggan
        OnDelivery,         // [Courier] Selesai dicuci, kurir sedang mengantar balik

        Completed,          // Selesai & sudah di tangan Customer
        Cancelled           // Dibatalkan
    }

    public enum PaymentStatus
    {
        Unpaid,
        Verifying,          // Customer sudah upload struk, menunggu dicek kasir
        Paid,
        Failed              // Kasir menolak struk (buram/salah)
    }

    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public Guid? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public User? Customer { get; set; }

        public bool IsManualOrder { get; set; } = false;
        
        [MaxLength(100)]
        public string? GuestCustomerName { get; set; }
        
        [MaxLength(20)]
        public string? GuestCustomerPhone { get; set; }

        [Required]
        public Guid StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        // Kolom untuk menyimpan pilihan metode pengiriman
        public DeliveryType DeliveryType { get; set; } = DeliveryType.SelfService;

        public string? DeliveryAddress { get; set; }
        public double? DeliveryLatitude { get; set; }
        public double? DeliveryLongitude { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public string? PaymentReceiptUrl { get; set; }
        public string? PaymentRejectionReason { get; set; }

        public Guid? SelectedStoreBankAccountId { get; set; }
        [ForeignKey("SelectedStoreBankAccountId")]
        public StoreBankAccount? SelectedStoreBankAccount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryFee { get; set; } = 0;

        [MaxLength(50)]
        public string? AppliedPromoCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.PayLater;
        public string? FinalPaymentProvider { get; set; } // Menyimpan metode aktual (Tunai/QRIS) saat transaksi ditutup
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedCompletionDate { get; set; }
        public DateTime? PaidAt { get; set; }           // Kapan pembayaran diterima lunas
        public DateTime? CompletedAt { get; set; }
        public string? CancellationReason { get; set; } // Menyimpan alasan kenapa dibatalkan
        public DateTime? CancelledAt { get; set; }      // Kapan dibatalkan

        public Guid? PickupDriverId { get; set; }
        [ForeignKey("PickupDriverId")]
        public DriverProfile? PickupDriver { get; set; }

        public Guid? DeliveryDriverId { get; set; }
        [ForeignKey("DeliveryDriverId")]
        public DriverProfile? DeliveryDriver { get; set; }

        [MaxLength(50)]
        public string? PickupTimeSlot { get; set; }

        [MaxLength(50)]
        public string? DeliveryTimeSlot { get; set; }

        public string? CustomerLaundryImageUrl { get; set; }

        [MaxLength(255)]
        public string? LogisticsNotes { get; set; }

        [MaxLength(10)]
        public string? OTPCode { get; set; }

        public string? PickupEvidenceUrl { get; set; }
        public string? DeliveryEvidenceUrl { get; set; }
        public bool IsSettledToStore { get; set; } = false;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}