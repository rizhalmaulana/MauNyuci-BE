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
                .Where(o => (o.PickupDriverId == driver.Id && o.Status == OrderStatus.OnPickup) || 
                            (o.DeliveryDriverId == driver.Id && o.Status == OrderStatus.OnDelivery))
                .ToListAsync();

            var result = orders.Select(o => 
            {
                double distance = 0;
                if (driver.CurrentLatitude.HasValue && driver.CurrentLongitude.HasValue && 
                    o.DeliveryLatitude.HasValue && o.DeliveryLongitude.HasValue)
                {
                    distance = CalculateDistanceInKm(driver.CurrentLatitude.Value, driver.CurrentLongitude.Value, 
                        o.DeliveryLatitude.Value, o.DeliveryLongitude.Value);
                }

                return new DriverTaskResponseDto
                {
                    OrderId = o.Id,
                    CustomerName = o.Customer?.FullName ?? "User",
                    Address = o.DeliveryAddress ?? "-",
                    TaskType = o.Status == OrderStatus.OnPickup ? "Pickup" : "Delivery",
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    DistanceInKm = Math.Round(distance, 2)
                };
            }).OrderBy(x => x.DistanceInKm).ToList();

            return result;
        }

        public async Task<DriverTaskDetailResponseDto> GetTaskDetailAsync(Guid userId, Guid orderId)
        {
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null) throw new Exception("Profil Driver tidak ditemukan.");

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId && 
                    ((o.PickupDriverId == driver.Id && o.Status == OrderStatus.OnPickup) || 
                     (o.DeliveryDriverId == driver.Id && o.Status == OrderStatus.OnDelivery)));

            if (order == null) throw new Exception("Tugas tidak ditemukan atau bukan milik Anda.");

            double distance = 0;
            if (driver.CurrentLatitude.HasValue && driver.CurrentLongitude.HasValue && 
                order.DeliveryLatitude.HasValue && order.DeliveryLongitude.HasValue)
            {
                distance = CalculateDistanceInKm(driver.CurrentLatitude.Value, driver.CurrentLongitude.Value, 
                    order.DeliveryLatitude.Value, order.DeliveryLongitude.Value);
            }

            return new DriverTaskDetailResponseDto
            {
                OrderId = order.Id,
                CustomerName = order.Customer?.FullName ?? "User",
                CustomerPhone = order.Customer?.PhoneNumber ?? "-",
                CustomerPhotoUrl = order.Customer?.ProfilePictureUrl,
                Address = order.DeliveryAddress ?? "-",
                Latitude = order.DeliveryLatitude,
                Longitude = order.DeliveryLongitude,
                CourierNotes = order.LogisticsNotes,
                CustomerLaundryImageUrl = order.CustomerLaundryImageUrl,
                TaskType = order.Status == OrderStatus.OnPickup ? "Pickup" : "Delivery",
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                PickupTimeSlot = order.PickupTimeSlot,
                DeliveryTimeSlot = order.DeliveryTimeSlot,
                DistanceInKm = Math.Round(distance, 2)
            };
        }

        private double CalculateDistanceInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371d; // Radius of the earth in km
            var dLat = Deg2Rad(lat2 - lat1);  
            var dLon = Deg2Rad(lon2 - lon1); 
            var a = 
                Math.Sin(dLat/2) * Math.Sin(dLat/2) +
                Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * 
                Math.Sin(dLon/2) * Math.Sin(dLon/2); 
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a)); 
            return R * c; 
        }

        private double Deg2Rad(double deg)
        {
            return deg * (Math.PI/180d);
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
                .Where(o => o.DeliveryDriverId == driverId &&
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

        public async Task<IEnumerable<StoreDriverResponseDto>> GetStoreDriversAsync(Guid storeId, Guid requesterUserId)
        {
            var store = await _context.Store.FirstOrDefaultAsync(s => s.Id == storeId);
            if (store == null) throw new Exception("Toko tidak ditemukan.");

            // Owner atau staff aktif toko tersebut saja yang boleh melihat daftar driver
            bool hasAccess = store.OwnerId == requesterUserId ||
                await _context.StoreStaffs.AnyAsync(s => s.StoreId == storeId && s.UserId == requesterUserId && s.IsActive);
            if (!hasAccess)
                throw new UnauthorizedAccessException("Akses ditolak! Anda tidak memiliki izin untuk melihat driver toko ini.");

            var drivers = await _context.DriverProfiles
                .Include(d => d.User)
                .Where(d => d.StoreId == storeId)
                .ToListAsync();

            // Hitung beban tugas aktif per driver agar FE bisa menampilkan status sibuk
            var driverIds = drivers.Select(d => d.Id).ToList();
            var activeOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.OnPickup || o.Status == OrderStatus.OnDelivery)
                .Where(o => (o.PickupDriverId != null && driverIds.Contains(o.PickupDriverId.Value)) ||
                            (o.DeliveryDriverId != null && driverIds.Contains(o.DeliveryDriverId.Value)))
                .Select(o => new { o.Status, o.PickupDriverId, o.DeliveryDriverId })
                .ToListAsync();

            return drivers.Select(d => new StoreDriverResponseDto
            {
                DriverId = d.Id, // INI yang dikirim FE sebagai DriverId saat confirm-pickup / ready-for-delivery
                UserId = d.UserId,
                FullName = d.User?.FullName ?? "-",
                PhoneNumber = d.User?.PhoneNumber ?? "-",
                VehicleNumber = d.VehicleNumber,
                VehicleType = d.VehicleType,
                IsAvailable = d.IsAvailable,
                ActiveTaskCount = activeOrders.Count(o =>
                    (o.Status == OrderStatus.OnPickup && o.PickupDriverId == d.Id) ||
                    (o.Status == OrderStatus.OnDelivery && o.DeliveryDriverId == d.Id))
            }).ToList();
        }

        private OrderResponseDto MapToResponse(Order o)
        {
            // Logika mapping sama dengan di OrderService
            return new OrderResponseDto { Id = o.Id, Status = o.Status.ToString() };
        }
        public async Task UpdateProfileAsync(Guid userId, UpdateDriverProfileDto request)
        {
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null) throw new Exception("Profil driver tidak ditemukan.");

            if (request.VehicleNumber != null)
                driver.VehicleNumber = request.VehicleNumber;
            
            if (request.VehicleType != null)
                driver.VehicleType = request.VehicleType;

            if (request.IsAvailable.HasValue)
                driver.IsAvailable = request.IsAvailable.Value;

            driver.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<DriverProfileResponseDto> GetProfileAsync(Guid userId)
        {
            var driver = await _context.DriverProfiles
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null) throw new Exception("Profil driver tidak ditemukan.");

            // Hitung UnsettledCash
            var unsettledOrders = await _context.Orders
                .Where(o => o.DeliveryDriverId == driver.Id &&
                           o.PaymentMethod == PaymentMethod.PayLater &&
                           o.PaymentStatus == PaymentStatus.Paid &&
                           o.IsSettledToStore == false)
                .ToListAsync();
            
            decimal unsettledCash = unsettledOrders.Sum(o => o.TotalAmount);

            return new DriverProfileResponseDto
            {
                Id = driver.Id,
                Name = driver.User?.FullName ?? "Unknown",
                Email = driver.User?.Email,
                PhoneNumber = driver.User?.PhoneNumber ?? string.Empty,
                VehiclePlateNumber = driver.VehicleNumber,
                VehicleType = driver.VehicleType,
                IsAvailable = driver.IsAvailable,
                UnsettledCash = unsettledCash,
                PhotoUrl = driver.User?.ProfilePictureUrl
            };
        }

        public async Task UpdateStatusAsync(Guid userId, bool isAvailable)
        {
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null) throw new Exception("Profil driver tidak ditemukan.");

            driver.IsAvailable = isAvailable;
            driver.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<DriverTaskHistoryResponseDto> GetTaskHistoryAsync(Guid userId, DateTime? startDate, DateTime? endDate)
        {
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null) throw new Exception("Profil driver tidak ditemukan.");

            var query = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.Status == OrderStatus.Completed && 
                           (o.PickupDriverId == driver.Id || o.DeliveryDriverId == driver.Id));

            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                query = query.Where(o => o.CompletedAt >= start);
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(o => o.CompletedAt <= end);
            }

            var orders = await query.ToListAsync();

            var tasks = new List<DriverTaskHistoryItemDto>();
            foreach (var o in orders)
            {
                // Jika driver ini yang pickup
                if (o.PickupDriverId == driver.Id)
                {
                    tasks.Add(new DriverTaskHistoryItemDto
                    {
                        TaskId = o.Id, // We use orderId as TaskId for simplicity
                        TransactionId = "TRX-" + o.Id.ToString().Substring(0, 8).ToUpper(),
                        TaskType = "Pickup",
                        CustomerName = o.Customer?.FullName ?? o.GuestCustomerName ?? "Unknown",
                        CustomerAddress = o.DeliveryAddress ?? "-",
                        CompletedAt = o.CompletedAt ?? o.CreatedAt,
                        TotalAmount = o.TotalAmount,
                        PaymentMethod = o.PaymentMethod.ToString()
                    });
                }

                // Jika driver ini yang delivery
                if (o.DeliveryDriverId == driver.Id)
                {
                    tasks.Add(new DriverTaskHistoryItemDto
                    {
                        TaskId = o.Id, 
                        TransactionId = "TRX-" + o.Id.ToString().Substring(0, 8).ToUpper(),
                        TaskType = "Delivery",
                        CustomerName = o.Customer?.FullName ?? o.GuestCustomerName ?? "Unknown",
                        CustomerAddress = o.DeliveryAddress ?? "-",
                        CompletedAt = o.CompletedAt,
                        TotalAmount = o.TotalAmount,
                        PaymentMethod = o.PaymentMethod.ToString()
                    });
                }
            }

            // Sort descending by date
            tasks = tasks.OrderByDescending(t => t.CompletedAt).ToList();

            // Calculate cash collected
            // Cash is collected only on Delivery if PaymentMethod == PayLater
            var cashOrders = orders.Where(o => o.DeliveryDriverId == driver.Id && o.PaymentMethod == PaymentMethod.PayLater).ToList();
            
            var summary = new DriverTaskHistorySummaryDto
            {
                TotalTasks = tasks.Count,
                TotalPickup = tasks.Count(t => t.TaskType == "Pickup"),
                TotalDelivery = tasks.Count(t => t.TaskType == "Delivery"),
                TotalCashCollected = cashOrders.Sum(o => o.TotalAmount)
            };

            return new DriverTaskHistoryResponseDto
            {
                Summary = summary,
                Tasks = tasks
            };
        }
    }
}