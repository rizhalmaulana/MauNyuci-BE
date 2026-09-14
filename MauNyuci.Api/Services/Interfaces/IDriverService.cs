using MauNyuci.Api.DTOs.Driver;
using MauNyuci.Api.DTOs.Order;
using MauNyuci.Api.Models;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IDriverService
    {
        // Melihat daftar jemputan (Status: OnPickup) atau antaran (Status: OnDelivery)
        Task<IEnumerable<DriverTaskResponseDto>> GetMyTasksAsync(Guid userId);

        // Kasus Penjemputan: Driver tiba di rumah customer dan mengambil baju
        Task<OrderResponseDto> ConfirmPickupAsync(Guid orderId, Guid userId, IFormFile evidenceFile);

        // Kasus Pengantaran + Bayar Tunai (COD): Driver terima uang fisik
        Task<OrderResponseDto> ConfirmDeliveryAndCashPaymentAsync(Guid orderId, Guid userId);

        // Update lokasi GPS Driver (untuk tracking di HP Customer)
        Task UpdateLocationAsync(Guid userId, double lat, double lng);
        Task<OrderResponseDto> ConfirmDeliveryWithPhotoAsync(Guid orderId, Guid userId, IFormFile evidenceFile, string note);
        Task<DriverSettlement> SettleCashToStoreAsync(Guid driverId, Guid storeOwnerId);
    }
}