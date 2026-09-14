using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs.Driver;
using MauNyuci.Api.DTOs.Order;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Services.Implementations
{
    public class DriverService : IDriverService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMediaService _mediaService; // Untuk upload foto bukti ke Cloudflare R2
        private readonly AppDbContext _context; // Untuk akses langsung DriverProfile

        public DriverService(IOrderRepository orderRepository, IMediaService mediaService, AppDbContext context)
        {
            _orderRepository = orderRepository;
            _mediaService = mediaService;
            _context = context;
        }

        public async Task<IEnumerable<DriverTaskResponseDto>> GetMyTasksAsync(Guid userId)
        {
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null) throw new Exception("Profil Driver tidak ditemukan.");

            // Ambil orderan yang di-assign ke driver ini dan belum selesai
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.DriverId == driver.Id &&
                           (o.Status == OrderStatus.OnPickup || o.Status == OrderStatus.OnDelivery))
                .ToListAsync();

            return orders.Select(o => new DriverTaskResponseDto
            {
                OrderId = o.Id,
                CustomerName = o.Customer?.FullName ?? "User",
                CustomerPhone = o.Customer?.PhoneNumber ?? "-",
                Address = o.DeliveryAddress ?? "-",
                TaskType = o.Status == OrderStatus.OnPickup ? "Pickup" : "Delivery",
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                PaymentMethod = o.PaymentMethod.ToString(),
                PaymentStatus = o.PaymentStatus.ToString()
            });
        }

        public async Task<OrderResponseDto> ConfirmPickupAsync(Guid orderId, Guid userId, IFormFile evidenceFile)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Order tidak ditemukan.");

            // Upload foto bukti ke Cloudflare R2
            var evidenceUrl = await _mediaService.UploadImageAsync(evidenceFile, "pickup-evidence");

            order.PickupEvidenceUrl = evidenceUrl;
            order.Status = OrderStatus.Confirmed; // Pastikan baju sudah diambil driver, status berubah jadi Confirmed (siap ditimbang di toko)

            await _orderRepository.UpdateOrderAsync(order);
            return MapToResponse(order);
        }

        public async Task<OrderResponseDto> ConfirmDeliveryAndCashPaymentAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Order tidak ditemukan.");

            // LOGIKA KRUSIAL: Jika Bayar Tunai (PayLater)
            if (order.PaymentMethod == PaymentMethod.PayLater)
            {
                order.PaymentStatus = PaymentStatus.Paid;
            }

            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;

            await _orderRepository.UpdateOrderAsync(order);
            return MapToResponse(order);
        }

        public async Task UpdateLocationAsync(Guid userId, double lat, double lng)
        {
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver != null)
            {
                driver.CurrentLatitude = lat;
                driver.CurrentLongitude = lng;
                driver.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<OrderResponseDto> ConfirmDeliveryWithPhotoAsync(Guid orderId, Guid userId, IFormFile evidenceFile, string note)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Order tidak ditemukan.");

            // Upload foto bukti ke Cloudflare R2
            var evidenceUrl = await _mediaService.UploadImageAsync(evidenceFile, "delivery-evidence");

            order.DeliveryEvidenceUrl = evidenceUrl;
            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(note))
            {
                order.CancellationReason = $"[NOTE DRIVER]: {note}";
            }

            await _orderRepository.UpdateOrderAsync(order);
            return MapToResponse(order);
        }

        public async Task<DriverSettlement> SettleCashToStoreAsync(Guid driverId, Guid storeOwnerId)
        {
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.Id == driverId);
            if (driver == null) throw new Exception("Driver tidak ditemukan.");

            // Validasi Keamanan: Pastikan yang nge-klik "Terima Setoran" adalah Owner tokonya
            var store = await _context.Store.FirstOrDefaultAsync(s => s.Id == driver.StoreId);
            if (store == null || store.OwnerId != storeOwnerId)
                throw new UnauthorizedAccessException("Anda tidak memiliki akses ke kasir toko ini.");

            // Ambil semua orderan PayLater yang dibawa driver ini tapi belum disetor
            var unsettledOrders = await _context.Orders
                .Where(o => o.DriverId == driverId &&
                           o.PaymentMethod == PaymentMethod.PayLater &&
                           o.PaymentStatus == PaymentStatus.Paid &&
                           o.IsSettledToStore == false)
                .ToListAsync();

            if (!unsettledOrders.Any())
                throw new Exception("Tidak ada tagihan uang tunai yang perlu disetor oleh driver ini.");

            decimal totalSettlement = unsettledOrders.Sum(o => o.TotalAmount);

            // 1. Ubah status orderan menjadi 'Sudah Disetor'
            foreach (var order in unsettledOrders)
            {
                order.IsSettledToStore = true;
                _context.Orders.Update(order);
            }

            // 2. Buat riwayat setoran di tabel DriverSettlement
            var settlement = new DriverSettlement
            {
                DriverId = driverId,
                StoreId = driver.StoreId,
                TotalAmount = totalSettlement,
                VerifiedByAdminId = storeOwnerId,
                SettledAt = DateTime.UtcNow
            };

            await _context.DriverSettlements.AddAsync(settlement);
            await _context.SaveChangesAsync();

            return settlement;
        }

        private OrderResponseDto MapToResponse(Order o)
        {
            // Logika mapping sama dengan di OrderService
            return new OrderResponseDto { Id = o.Id, Status = o.Status.ToString() };
        }
    }
}