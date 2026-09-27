using MauNyuci.Api.DTOs.Order;
using MauNyuci.Api.DTOs;
namespace MauNyuci.Api.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(Guid customerId, OrderCreateRequestDto request);
        Task<OrderResponseDto> CreatePosOrderAsync(Guid userId, OrderPosRequestDto request);
        Task<OrderResponseDto> ConfirmPickupAsync(Guid orderId, Guid storeOwnerId, Guid pickupDriverId);
        Task<OrderResponseDto> UpdateWeightAsync(Guid orderId, OrderConfirmRequestDto request, Guid storeOwnerId);
        Task<OrderResponseDto> ChangePaymentMethodAsync(Guid orderId, Guid customerId, string newMethod);
        Task<OrderResponseDto> ReadyForDeliveryAsync(Guid orderId, Guid storeOwnerId, Guid deliveryDriverId);
        // Fungsi Toko: Menerima Orderan dan siap diproses
        Task<OrderResponseDto> AcceptOrderAsync(Guid orderId, Guid userId);

        // Fungsi Toko: Menolak Orderan dan kembali ke status Cancelled
        Task<OrderResponseDto> CancelOrderAsync(Guid orderId, Guid userId);

        // Fungsi Toko: Menandai cucian sudah selesai diproses (siap diambil/diantar)
        Task<OrderResponseDto> FinishWashingAsync(Guid orderId, Guid userId);

        // Fungsi Toko/Kurir: Menyerahkan baju ke Customer dan memastikan pembayaran lunas
        Task<OrderResponseDto> CompleteOrderAsync(Guid orderId, Guid userId, OrderCompleteRequestDto request);

        // Fungsi Customer: Menampilkan daftar pesanan milik 1 Customer
        Task<PagedResponseDto<OrderHeaderResponseDto>> GetCustomerOrdersAsync(Guid customerId, string? status, int page, int pageSize);
        Task<IEnumerable<OrderHeaderResponseDto>> GetStoreOrdersAsync(Guid storeId, Guid userId);
        Task<PagedResponseDto<OrderHeaderResponseDto>> GetStoreOrderHistoryAsync(Guid storeId, Guid userId, string? search, DateTime? startDate, DateTime? endDate, int page, int pageSize);
        Task<StoreTransactionsSummaryDto> GetStoreTransactionsSummaryAsync(Guid storeId, Guid userId, DateTime? startDate, DateTime? endDate);

        // Melihat detail 1 struk pesanan (Bisa dipakai oleh Toko maupun Customer)
        Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId, Guid userId);

        Task<OrderResponseDto> CustomerCancelOrderAsync(Guid orderId, Guid customerId, OrderCancelRequestDto request);

        // Upload Struk
        Task<OrderResponseDto> UploadPaymentReceiptAsync(Guid orderId, Guid customerId, IFormFile receiptFile);
        Task<OrderResponseDto> StoreUploadPaymentReceiptAsync(Guid orderId, Guid storeOwnerId, IFormFile receiptFile);
        
        // Verifikasi Pembayaran oleh Toko(Terima/Tolak)
        Task<OrderResponseDto> VerifyPaymentAsync(Guid orderId, Guid storeOwnerId, VerifyPaymentRequestDto request);

        // Fungsi Driver: Konfirmasi terima uang tunai (COD)
        Task<OrderResponseDto> ConfirmDriverCashPaymentAsync(Guid orderId, Guid driverId);
        Task<decimal> GetUnsettledCashForDriverAsync(Guid userId);
    }
}