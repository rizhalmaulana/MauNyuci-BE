using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs;
using MauNyuci.Api.DTOs.Order;
using MauNyuci.Api.Hubs;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Hangfire;

namespace MauNyuci.Api.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreCatalogRepository _catalogRepository;
        private readonly IStorePromoService _promoService;
        private readonly IMediaService _mediaService;
        private readonly AppDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly INotificationDispatcher _dispatcher;

        public OrderService(
            IOrderRepository orderRepository,
            IStoreRepository storeRepository,
            IStoreCatalogRepository catalogRepository,
            IStorePromoService promoService,
            IMediaService mediaService,
            AppDbContext context,
            IHubContext<OrderHub> hubContext,
            INotificationDispatcher dispatcher)
        {
            _orderRepository = orderRepository;
            _storeRepository = storeRepository;
            _catalogRepository = catalogRepository;
            _promoService = promoService;
            _mediaService = mediaService;
            _context = context;
            _hubContext = hubContext;
            _dispatcher = dispatcher;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(Guid customerId, OrderCreateRequestDto request)
        {
            // Validasi Toko
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                throw new Exception("Toko tidak ditemukan.");
            }

            // Buat Draft Struk Pesanan
            var newOrder = new Order
            {
                CustomerId = customerId,
                StoreId = request.StoreId,
                DeliveryType = request.DeliveryType,
                PaymentMethod = request.PaymentMethod,
                DeliveryAddress = request.DeliveryAddress,
                DeliveryLatitude = request.DeliveryLatitude,
                DeliveryLongitude = request.DeliveryLongitude,
                PickupTimeSlot = request.PickupTimeSlot,
                DeliveryTimeSlot = request.DeliveryTimeSlot,
                CustomerLaundryImageUrl = request.CustomerLaundryImageUrl,
                LogisticsNotes = request.LogisticsNotes,
                OTPCode = new Random().Next(100000, 999999).ToString(),
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Unpaid,
                CreatedAt = DateTime.UtcNow
            };

            // Tentukan Biaya Ongkir
            // Jika pilih dijemput kurir, masukkan tarif antar-jemput dari profil toko
            if (request.DeliveryType == DeliveryType.Courier)
            {
                newOrder.DeliveryFee = store.PickupDeliveryFee; // Mengambil tarif kurir milik toko
            }

            // Proses Rincian Layanan (Order Items) & Ambil Snapshot Harga
            decimal totalLayanan = 0;
            int maxDurationInHours = 0;

            var catalogItems = await _catalogRepository.GetByStoreIdAsync(request.StoreId);

            foreach (var itemRequest in request.Items)
            {
                var catalogItem = catalogItems.FirstOrDefault(c => c.Id == itemRequest.CatalogItemId);
                if (catalogItem == null) continue;

                if (itemRequest.Quantity < 0) throw new Exception("Kuantitas tidak valid.");

                // Hitung subtotal awal
                var subTotal = catalogItem.Price * itemRequest.Quantity;

                int itemDuration = ParseTimeEstimateToHours(catalogItem.TimeEstimate);
                if (itemDuration > maxDurationInHours)
                {
                    maxDurationInHours = itemDuration;
                }

                var orderItem = new OrderItem
                {
                    CatalogItemId = catalogItem.Id,
                    ItemName = catalogItem.Name,
                    UnitPrice = catalogItem.Price,
                    Unit = catalogItem.Unit,
                    Quantity = itemRequest.Quantity,
                    SubTotal = subTotal
                };

                newOrder.OrderItems.Add(orderItem);
                totalLayanan += subTotal;
            }

            // --- PROMO LOGIC ---
            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                var promo = await _promoService.ValidatePromoAsync(request.StoreId, request.PromoCode, totalLayanan);
                
                decimal maxTargetAmount = 0;
                if (promo!.DiscountTarget == "Service") maxTargetAmount = totalLayanan;
                else if (promo!.DiscountTarget == "Delivery") maxTargetAmount = newOrder.DeliveryFee;
                else maxTargetAmount = totalLayanan + newOrder.DeliveryFee;

                decimal discount = 0;
                if (promo!.DiscountType == "Nominal")
                {
                    discount = promo!.DiscountValue;
                }
                else if (promo!.DiscountType == "Percentage")
                {
                    discount = maxTargetAmount * (promo!.DiscountValue / 100);
                    if (promo!.MaxDiscountAmount.HasValue && discount > promo!.MaxDiscountAmount.Value)
                    {
                        discount = promo!.MaxDiscountAmount.Value;
                    }
                }

                if (discount > maxTargetAmount) discount = maxTargetAmount; // Cap to max target

                newOrder.DiscountAmount = discount;
                newOrder.AppliedPromoCode = promo!.PromoCode;
            }

            // Kalkulasi Total Keseluruhan
            newOrder.TotalAmount = totalLayanan + newOrder.DeliveryFee - newOrder.DiscountAmount;

            if (maxDurationInHours > 0)
            {
                newOrder.ExpectedCompletionDate = DateTime.UtcNow.AddHours(maxDurationInHours);
            }

            // Simpan ke Database
            var createdOrder = await _orderRepository.CreateOrderAsync(newOrder);

            // SignalR Trigger
            await _hubContext.Clients.Group($"Store-{store.Id}").SendAsync("DashboardUpdated");

            // Notification: Immediate
            await _dispatcher.DispatchNotificationAsync(store.OwnerId, "Pesanan Baru Masuk!", "Ada pesanan baru yang menunggu konfirmasi.");

            // Notification: Scheduled Payment Reminder (30 Minutes)
            if (createdOrder.PaymentMethod == PaymentMethod.PayNow)
            {
                BackgroundJob.Schedule<IReminderJobService>(x => x.SendPaymentReminderAsync(createdOrder.Id), TimeSpan.FromMinutes(30));
            }

            return MapToResponseDto(createdOrder, store.Name);
        }

        public async Task<OrderResponseDto> CreatePosOrderAsync(Guid userId, OrderPosRequestDto request)
        {
            // Validasi Toko berdasarkan OwnerId atau StaffId
            var store = await _storeRepository.GetByOwnerOrStaffIdAsync(userId);
            if (store == null)
            {
                throw new Exception("Toko tidak ditemukan.");
            }

            // Buat Draft Struk Pesanan Manual
            var newOrder = new Order
            {
                CustomerId = null, // Kosongkan karena bukan user terdaftar
                IsManualOrder = true,
                GuestCustomerName = request.GuestCustomerName,
                GuestCustomerPhone = request.GuestCustomerPhone,
                StoreId = store.Id,
                DeliveryType = request.DeliveryType,
                PaymentMethod = request.PaymentMethod,
                DeliveryAddress = request.DeliveryAddress,
                DeliveryLatitude = request.DeliveryLatitude,
                DeliveryLongitude = request.DeliveryLongitude,
                PickupTimeSlot = request.PickupTimeSlot,
                DeliveryTimeSlot = request.DeliveryTimeSlot,
                LogisticsNotes = request.LogisticsNotes,
                OTPCode = new Random().Next(100000, 999999).ToString(),
                SelectedStoreBankAccountId = request.SelectedStoreBankAccountId,
                Status = request.PaymentMethod == PaymentMethod.PayNow ? OrderStatus.AwaitingPayment : OrderStatus.Washing,
                PaymentStatus = PaymentStatus.Unpaid,
                CreatedAt = DateTime.UtcNow
            };

            // Tentukan Biaya Ongkir
            if (request.DeliveryType == DeliveryType.Courier)
            {
                newOrder.DeliveryFee = store.PickupDeliveryFee;
            }

            // Proses Rincian Layanan (Order Items) & Ambil Snapshot Harga
            decimal totalLayanan = 0;
            int maxDurationInHours = 0;

            var catalogItems = await _catalogRepository.GetByStoreIdAsync(store.Id);

            foreach (var itemRequest in request.Items)
            {
                var catalogItem = catalogItems.FirstOrDefault(c => c.Id == itemRequest.CatalogItemId);
                if (catalogItem == null) continue;

                if (itemRequest.Quantity < 0) throw new Exception("Kuantitas tidak valid.");

                var subTotal = catalogItem.Price * itemRequest.Quantity;

                int itemDuration = ParseTimeEstimateToHours(catalogItem.TimeEstimate);
                if (itemDuration > maxDurationInHours)
                {
                    maxDurationInHours = itemDuration;
                }

                var orderItem = new OrderItem
                {
                    CatalogItemId = catalogItem.Id,
                    ItemName = catalogItem.Name,
                    UnitPrice = catalogItem.Price,
                    Unit = catalogItem.Unit,
                    Quantity = itemRequest.Quantity,
                    SubTotal = subTotal
                };

                newOrder.OrderItems.Add(orderItem);
                totalLayanan += subTotal;
            }

            // --- PROMO LOGIC ---
            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                var promo = await _promoService.ValidatePromoAsync(store.Id, request.PromoCode, totalLayanan);
                
                decimal maxTargetAmount = 0;
                if (promo!.DiscountTarget == "Service") maxTargetAmount = totalLayanan;
                else if (promo!.DiscountTarget == "Delivery") maxTargetAmount = newOrder.DeliveryFee;
                else maxTargetAmount = totalLayanan + newOrder.DeliveryFee;

                decimal discount = 0;
                if (promo!.DiscountType == "Nominal")
                {
                    discount = promo!.DiscountValue;
                }
                else if (promo!.DiscountType == "Percentage")
                {
                    discount = maxTargetAmount * (promo!.DiscountValue / 100);
                    if (promo!.MaxDiscountAmount.HasValue && discount > promo!.MaxDiscountAmount.Value)
                    {
                        discount = promo!.MaxDiscountAmount.Value;
                    }
                }

                if (discount > maxTargetAmount) discount = maxTargetAmount; // Cap to max target

                newOrder.DiscountAmount = discount;
                newOrder.AppliedPromoCode = promo!.PromoCode;
            }

            // Kalkulasi Total Keseluruhan
            newOrder.TotalAmount = totalLayanan + newOrder.DeliveryFee - newOrder.DiscountAmount;

            if (maxDurationInHours > 0)
            {
                newOrder.ExpectedCompletionDate = DateTime.UtcNow.AddHours(maxDurationInHours);
            }

            // Simpan ke Database
            var createdOrder = await _orderRepository.CreateOrderAsync(newOrder);

            // SignalR Trigger
            await _hubContext.Clients.Group($"Store-{store.Id}").SendAsync("DashboardUpdated");

            return MapToResponseDto(createdOrder, store.Name);
        }

        public async Task<OrderResponseDto> UpdateWeightAsync(Guid orderId, OrderConfirmRequestDto request, Guid storeOwnerId)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null) throw new Exception("Pesanan tidak ditemukan.");

                await ValidateStoreOwnershipAsync(order.StoreId, storeOwnerId);

                var allowedStatuses = new[] { OrderStatus.WaitingForDropOff, OrderStatus.OnPickup, OrderStatus.Confirmed };
                if (!allowedStatuses.Contains(order.Status))
                    throw new Exception("Pesanan belum siap untuk ditimbang.");

                decimal totalLayananBaru = 0;

                foreach (var inputItem in request.Items)
                {
                    if (inputItem.ActualQuantity <= 0) throw new Exception("Berat/Jumlah harus lebih dari 0.");

                    var existingItem = order.OrderItems.FirstOrDefault(i => i.Id == inputItem.OrderItemId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity = inputItem.ActualQuantity;
                        existingItem.SubTotal = existingItem.UnitPrice * inputItem.ActualQuantity;
                    }
                }

                foreach (var item in order.OrderItems)
                {
                    totalLayananBaru += item.SubTotal;
                }

                order.TotalAmount = totalLayananBaru + order.DeliveryFee - order.DiscountAmount;

                if (order.PaymentMethod == PaymentMethod.PayNow)
                {
                    order.Status = OrderStatus.AwaitingPayment;
                }
                else
                {
                    order.Status = OrderStatus.Washing;
                }

                var updatedOrder = await _orderRepository.UpdateOrderAsync(order);
                await transaction.CommitAsync();

                await _hubContext.Clients.Group(orderId.ToString())
                    .SendAsync("OrderStatusUpdated", new
                    {
                        OrderId = orderId,
                        NewStatus = updatedOrder.Status.ToString(),
                        TotalAmount = updatedOrder.TotalAmount
                    });

                if (updatedOrder.Status == OrderStatus.Washing && updatedOrder.ExpectedCompletionDate.HasValue)
                {
                    var timeUntilCompletion = updatedOrder.ExpectedCompletionDate.Value - DateTime.UtcNow;
                    var delay = timeUntilCompletion - TimeSpan.FromDays(1);
                    if (delay > TimeSpan.Zero)
                    {
                        BackgroundJob.Schedule<IReminderJobService>(x => x.SendStoreSLAReminderAsync(updatedOrder.Id), delay);
                    }
                }

                return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            });
        }

        public async Task<OrderResponseDto> AcceptOrderAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            await ValidateStoreOwnershipAsync(order.StoreId, userId);

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Hanya pesanan berstatus Pending yang dapat diterima.");

            if (order.DeliveryType == DeliveryType.Courier)
            {
                throw new Exception("Pesanan Antar-Jemput harus menggunakan fitur Penugasan Kurir.");
            }
            
            order.Status = OrderStatus.WaitingForDropOff; // Sinyal customer harus jalan ke toko

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            await _hubContext.Clients.Group(orderId.ToString())
                .SendAsync("OrderStatusUpdated", new
                {
                    OrderId = orderId,
                    NewStatus = updatedOrder.Status.ToString()
                });

            // Beritahu Customer bahwa pesanannya telah diterima oleh Toko
            if (updatedOrder.CustomerId.HasValue)
            {
                await _dispatcher.DispatchNotificationAsync(
                    updatedOrder.CustomerId.Value, 
                    "Pesanan Diterima", 
                    "Pesanan Anda telah dikonfirmasi oleh toko dan akan segera diproses.");
            }

            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<OrderResponseDto> CancelOrderAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            await ValidateStoreOwnershipAsync(order.StoreId, userId);

            // Bisa dibatalkan jika masih Pending atau baru Confirmed
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                throw new Exception("Pesanan pada tahap ini sudah tidak dapat dibatalkan.");

            order.Status = OrderStatus.Cancelled;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);
            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<OrderResponseDto> FinishWashingAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            await ValidateStoreOwnershipAsync(order.StoreId, userId);

            if (order.Status != OrderStatus.Washing)
                throw new Exception("Hanya pesanan berstatus Washing yang bisa diselesaikan proses cucinya.");

            if (order.DeliveryType == DeliveryType.Courier)
            {
                throw new Exception("Untuk pesanan Antar-Jemput, gunakan penugasan kurir pengantar.");
            }
            
            order.Status = OrderStatus.ReadyForPickup;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            await _hubContext.Clients.Group(orderId.ToString())
                .SendAsync("OrderStatusUpdated", new
                {
                    OrderId = orderId,
                    NewStatus = updatedOrder.Status.ToString()
                });

            await _hubContext.Clients.Group($"Store-{order.StoreId}").SendAsync("DashboardUpdated");

            // Beritahu Customer
            if (updatedOrder.CustomerId.HasValue)
            {
                await _dispatcher.DispatchNotificationAsync(updatedOrder.CustomerId.Value, "Cucian Selesai!", "Pesanan Anda sudah selesai dicuci dan siap!");
            }

            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<OrderResponseDto> CompleteOrderAsync(Guid orderId, Guid userId, OrderCompleteRequestDto request)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            await ValidateStoreOwnershipAsync(order.StoreId, userId);

            // Baju harus sudah siap diambil atau sedang diantar
            if (order.Status != OrderStatus.ReadyForPickup && order.Status != OrderStatus.OnDelivery)
                throw new Exception("Pesanan belum siap diselesaikan.");

            // GATEKEEPER PEMBAYARAN 
            if (order.PaymentMethod == PaymentMethod.PayNow && order.PaymentStatus != PaymentStatus.Paid)
            {
                throw new Exception("Pesanan Transfer tidak dapat diselesaikan karena pembayaran belum lunas/terverifikasi.");
            }

            if (order.PaymentMethod == PaymentMethod.PayLater)
            {
                // ANTI-FRAUD: Jika Kasir mengklaim dibayar secara Non-Tunai, wajib ada bukti bayar
                if (!request.FinalPaymentProvider.Equals("Tunai", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(order.PaymentReceiptUrl))
                    {
                        throw new Exception("Bukti bayar Non-Tunai harus di-upload terlebih dahulu sebelum menyelesaikan pesanan.");
                    }
                }
            }

            // Simpan metode aktual di database
            order.FinalPaymentProvider = request.FinalPaymentProvider;

            // Ubah status pembayaran menjadi Paid (Lunas) karena baju sudah diserahkan
            if (order.PaymentStatus != PaymentStatus.Paid)
            {
                order.PaymentStatus = PaymentStatus.Paid;
                order.PaidAt = DateTime.UtcNow;
            }

            // Tutup transaksi
            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow; // Catat waktu selesainya

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            await _hubContext.Clients.Group(orderId.ToString())
                .SendAsync("OrderStatusUpdated", new
                {
                    OrderId = orderId,
                    NewStatus = updatedOrder.Status.ToString(),
                    PaymentStatus = updatedOrder.PaymentStatus.ToString() // Update status lunas
                });

            await _hubContext.Clients.Group($"Store-{order.StoreId}").SendAsync("DashboardUpdated");

            // Notification: Review Reminder (15 Minutes)
            BackgroundJob.Schedule<IReminderJobService>(x => x.SendReviewReminderAsync(updatedOrder.Id), TimeSpan.FromMinutes(15));
            
            if (updatedOrder.CustomerId.HasValue)
            {
                await _dispatcher.DispatchNotificationAsync(updatedOrder.CustomerId.Value, "Pesanan Selesai", "Terima kasih telah menggunakan jasa kami!");
            }

            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<PagedResponseDto<OrderHeaderResponseDto>> GetCustomerOrdersAsync(Guid customerId, string? status, int page, int pageSize)
        {
            var result = await _orderRepository.GetByCustomerIdAsync(customerId, status, page, pageSize);
            var mappedData = result.Data.Select(o => MapToHeaderResponseDto(o, o.Store?.Name ?? "Toko")).ToList();

            return new PagedResponseDto<OrderHeaderResponseDto>
            {
                Data = mappedData,
                TotalItems = result.TotalItems,
                TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize),
                CurrentPage = page
            };
        }

        public async Task<IEnumerable<OrderHeaderResponseDto>> GetStoreOrdersAsync(Guid storeId, Guid userId)
        {
            await ValidateStoreOwnershipAsync(storeId, userId);

            // Jika aman, ambil data pesanannya
            var store = await _storeRepository.GetByIdAsync(storeId);
            var orders = await _orderRepository.GetByStoreIdAsync(storeId);
            return orders.Select(o => MapToHeaderResponseDto(o, store!.Name)).ToList();
        }

        public async Task<PagedResponseDto<OrderHeaderResponseDto>> GetStoreOrderHistoryAsync(Guid storeId, Guid userId, string? search, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            await ValidateStoreOwnershipAsync(storeId, userId);

            var store = await _storeRepository.GetByIdAsync(storeId);
            var result = await _orderRepository.GetHistoryByStoreIdAsync(storeId, search, startDate, endDate, page, pageSize);
            
            var mappedData = result.Data.Select(o => MapToHeaderResponseDto(o, store!.Name)).ToList();

            return new PagedResponseDto<OrderHeaderResponseDto>
            {
                Data = mappedData,
                TotalItems = result.TotalItems,
                TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize),
                CurrentPage = page
            };
        }

        public async Task<StoreTransactionsSummaryDto> GetStoreTransactionsSummaryAsync(Guid storeId, Guid userId, DateTime? startDate, DateTime? endDate)
        {
            await ValidateStoreOwnershipAsync(storeId, userId);
            return await _orderRepository.GetStoreTransactionsSummaryAsync(storeId, startDate, endDate);
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            // Cek apakah user yang request adalah Customer pemesan ATAU Owner toko
            bool isCustomer = order.CustomerId == userId;
            bool isStoreOwner = order.Store?.OwnerId == userId;

            // Jika dia bukan pemesan dan juga bukan pemilik toko, tolak aksesnya!
            if (!isCustomer && !isStoreOwner)
            {
                throw new UnauthorizedAccessException("Akses ditolak! Anda tidak memiliki izin untuk melihat detail pesanan ini.");
            }

            return MapToResponseDto(order, order.Store?.Name ?? "Toko");
        }

        private async Task ValidateStoreOwnershipAsync(Guid storeId, Guid userId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null) throw new Exception("Toko tidak ditemukan.");

            if (store.OwnerId != userId)
            {
                // Check if user is an active staff of THIS store
                var isStaff = await _context.StoreStaffs.AnyAsync(s => s.StoreId == storeId && s.UserId == userId && s.IsActive);
                if (!isStaff)
                {
                    throw new UnauthorizedAccessException("Akses ditolak! Anda tidak memiliki izin untuk memanipulasi pesanan di toko ini.");
                }
            }
        }

        // Customer Cancel Order
        public async Task<OrderResponseDto> CustomerCancelOrderAsync(Guid orderId, Guid customerId, OrderCancelRequestDto request)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            // Validasi Keamanan: Pastikan yang membatalkan adalah pemilik struk
            if (order.CustomerId != customerId)
                throw new UnauthorizedAccessException("Anda tidak berhak membatalkan pesanan ini.");

            // Validasi Status: Hanya bisa batal jika toko belum terima (masih Pending)
            if (order.Status != OrderStatus.Pending)
                throw new Exception("Pesanan sudah diproses oleh toko dan tidak dapat dibatalkan.");

            // Catat pembatalannya
            order.Status = OrderStatus.Cancelled;
            order.CancellationReason = request.Reason;
            order.CancelledAt = DateTime.UtcNow;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            // Notify Store
            if (updatedOrder.Store != null)
            {
                await _dispatcher.DispatchNotificationAsync(updatedOrder.Store.OwnerId, "Pesanan Dibatalkan", $"Pelanggan membatalkan pesanan. Alasan: {request.Reason}");
            }

            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<OrderResponseDto> UploadPaymentReceiptAsync(Guid orderId, Guid customerId, IFormFile receiptFile)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");
            if (order.CustomerId != customerId) throw new UnauthorizedAccessException("Ini bukan pesanan Anda.");
            if (order.Status != OrderStatus.AwaitingPayment && order.PaymentStatus != PaymentStatus.Failed)
                throw new Exception("Pesanan tidak dalam status menunggu pembayaran.");

            // Upload ke R2 (Asumsi Anda punya IMediaService. Jika namanya beda, sesuaikan)
            var receiptUrl = await _mediaService.UploadImageAsync(receiptFile, "receipts");

            order.PaymentReceiptUrl = receiptUrl;
            order.PaymentStatus = PaymentStatus.Verifying; // Berubah status agar kasir bisa cek

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            // SignalR Trigger
            await _hubContext.Clients.Group($"Store-{order.StoreId}").SendAsync("DashboardUpdated");

            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<OrderResponseDto> StoreUploadPaymentReceiptAsync(Guid orderId, Guid storeOwnerId, IFormFile receiptFile)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");
            
            await ValidateStoreOwnershipAsync(order.StoreId, storeOwnerId);

            if (order.PaymentStatus == PaymentStatus.Paid)
                throw new Exception("Pesanan sudah berstatus Lunas.");

            // Upload ke R2
            var receiptUrl = await _mediaService.UploadImageAsync(receiptFile, "receipts");

            order.PaymentReceiptUrl = receiptUrl;
            
            // Sesuai konfirmasi, langsung dianggap Lunas jika kasir yang upload
            order.PaymentStatus = PaymentStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            order.PaymentRejectionReason = null;
            
            // Lanjut ke proses cuci jika belum Washing/Selesai
            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.AwaitingPayment)
            {
                order.Status = OrderStatus.Washing;
            }

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            // SignalR Trigger
            await _hubContext.Clients.Group($"Store-{order.StoreId}").SendAsync("DashboardUpdated");

            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<OrderResponseDto> VerifyPaymentAsync(Guid orderId, Guid storeOwnerId, VerifyPaymentRequestDto request)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            await ValidateStoreOwnershipAsync(order.StoreId, storeOwnerId);

            if (order.PaymentStatus != PaymentStatus.Verifying && order.PaymentStatus != PaymentStatus.Unpaid)
                throw new Exception("Pesanan ini tidak sedang menunggu verifikasi pembayaran.");

            if (request.IsApproved)
            {
                order.PaymentStatus = PaymentStatus.Paid;
                order.PaidAt = DateTime.UtcNow;
                order.Status = OrderStatus.Washing; // Pembayaran sah, lanjut cuci!
                order.PaymentRejectionReason = null;
            }
            else
            {
                order.PaymentStatus = PaymentStatus.Failed;
                order.PaymentRejectionReason = request.RejectionReason ?? "Bukti transfer tidak valid/buram.";
            }

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            // SignalR Trigger
            await _hubContext.Clients.Group($"Store-{order.StoreId}").SendAsync("DashboardUpdated");

            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<OrderResponseDto> ConfirmDriverCashPaymentAsync(Guid orderId, Guid driverId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            // Validasi: Kurir yang login harus sama dengan kurir yang ditugaskan mengantar
            if (order.DeliveryDriverId != driverId)
                throw new UnauthorizedAccessException("Anda bukan kurir untuk pesanan ini.");

            // Validasi: Harus COD dan barang sedang diantar atau siap diambil
            if (order.PaymentMethod != PaymentMethod.PayLater)
                throw new Exception("Pesanan ini bukan pesanan COD/PayLater.");

            order.PaymentStatus = PaymentStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            order.Status = OrderStatus.Completed; // Selesaikan transaksi
            order.CompletedAt = DateTime.UtcNow;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);
            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<decimal> GetUnsettledCashForDriverAsync(Guid userId)
        {
            // Cari dulu DriverProfile-nya berdasarkan UserId dari login
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null) throw new Exception("Profil Driver tidak ditemukan.");

            // Hitung jumlah TotalAmount dari orderan yang:
            // - Dipegang oleh driver pengantar ini (DeliveryDriverId)
            // - Metode bayarnya Tunai/COD (PayLater)
            // - Status bayarnya sudah lunas (Paid)
            // - TAPI belum disetorkan ke toko (IsSettledToStore == false)
            var totalUnsettled = await _context.Orders
                .Where(o => o.DeliveryDriverId == driver.Id &&
                           o.PaymentMethod == PaymentMethod.PayLater &&
                           o.PaymentStatus == PaymentStatus.Paid &&
                           o.IsSettledToStore == false)
                .SumAsync(o => o.TotalAmount);

            return totalUnsettled;
        }

        // Fungsi bantuan untuk mengubah Model menjadi DTO yang rapi
        private OrderResponseDto MapToResponseDto(Order order, string storeName)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                StoreName = storeName,
                CustomerName = order.IsManualOrder ? (order.GuestCustomerName ?? "Guest") : (order.Customer?.FullName ?? "Customer"),
                IsManualOrder = order.IsManualOrder,
                GuestCustomerName = order.GuestCustomerName,
                GuestCustomerPhone = order.GuestCustomerPhone,
                CustomerPhone = order.IsManualOrder ? order.GuestCustomerPhone : order.Customer?.PhoneNumber,
                Status = order.Status.ToString(),
                DeliveryType = order.DeliveryType.ToString(),
                PaymentMethod = order.PaymentMethod.ToString(),
                TotalAmount = order.TotalAmount,
                DeliveryFee = order.DeliveryFee,
                AppliedPromoCode = order.AppliedPromoCode,
                DiscountAmount = order.DiscountAmount,
                PaymentStatus = order.PaymentStatus.ToString(),
                PaymentReceiptUrl = order.PaymentReceiptUrl,
                PaymentRejectionReason = order.PaymentRejectionReason,
                SelectedStoreBankAccountId = order.SelectedStoreBankAccountId,
                SelectedStoreBankName = order.SelectedStoreBankAccount?.BankName,
                SelectedStoreBankAccountNumber = order.SelectedStoreBankAccount?.AccountNumber,
                SelectedStoreAccountHolderName = order.SelectedStoreBankAccount?.AccountHolderName,
                CreatedAt = order.CreatedAt,
                ExpectedCompletionDate = order.ExpectedCompletionDate,
                IsLate = order.ExpectedCompletionDate.HasValue && order.ExpectedCompletionDate.Value < DateTime.UtcNow && order.Status != OrderStatus.Completed && order.Status != OrderStatus.ReadyForPickup && order.Status != OrderStatus.Cancelled,
                PaidAt = order.PaidAt,
                CustomerLaundryImageUrl = order.CustomerLaundryImageUrl,
                Items = order.OrderItems.Select(i => new OrderItemResponseDto
                {
                    OrderItemId = i.Id,
                    CatalogItemId = i.CatalogItemId,
                    ItemName = i.ItemName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    SubTotal = i.SubTotal,
                    ItemImageUrl = i.CatalogItem?.ImageAsset
                }).ToList()
            };
        }

        private OrderHeaderResponseDto MapToHeaderResponseDto(Order order, string storeName)
        {
            string? paymentProvider = null;
            if (order.PaymentMethod == PaymentMethod.PayLater) 
                paymentProvider = "Tunai/COD";
            else 
                paymentProvider = order.SelectedStoreBankAccount?.BankName;

            return new OrderHeaderResponseDto
            {
                Id = order.Id,
                StoreName = storeName,
                CustomerName = order.IsManualOrder ? (order.GuestCustomerName ?? "Guest") : (order.Customer?.FullName ?? "Customer"),
                IsManualOrder = order.IsManualOrder,
                GuestCustomerName = order.GuestCustomerName,
                CustomerPhone = order.IsManualOrder ? order.GuestCustomerPhone : order.Customer?.PhoneNumber,
                Status = order.Status.ToString(),
                DeliveryType = order.DeliveryType.ToString(),
                PaymentMethod = order.PaymentMethod.ToString(),
                TotalAmount = order.TotalAmount,
                TotalQuantity = order.OrderItems.Sum(i => i.Quantity),
                AppliedPromoCode = order.AppliedPromoCode,
                DiscountAmount = order.DiscountAmount,
                PaymentStatus = order.PaymentStatus.ToString(),
                PaymentProvider = paymentProvider,
                CreatedAt = order.CreatedAt,
                ExpectedCompletionDate = order.ExpectedCompletionDate,
                IsLate = order.ExpectedCompletionDate.HasValue && order.ExpectedCompletionDate.Value < DateTime.UtcNow && order.Status != OrderStatus.Completed && order.Status != OrderStatus.ReadyForPickup && order.Status != OrderStatus.Cancelled,
                PaidAt = order.PaidAt,
                ItemsSummary = order.OrderItems.Select(i => new OrderHeaderItemDto
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    UnitPrice = i.UnitPrice,
                    ItemImageUrl = i.CatalogItem?.ImageAsset
                }).ToList()
            };
        }

        private int ParseTimeEstimateToHours(string timeEstimate)
        {
            if (string.IsNullOrWhiteSpace(timeEstimate)) return 0;
            var parts = timeEstimate.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && int.TryParse(parts[0], out int value))
            {
                if (parts[1].Equals("Hari", StringComparison.OrdinalIgnoreCase)) return value * 24;
                if (parts[1].Equals("Jam", StringComparison.OrdinalIgnoreCase)) return value;
            }
            return 0; // Default fallback
        }
        public async Task<OrderResponseDto> ConfirmPickupAsync(Guid orderId, Guid storeOwnerId, Guid pickupDriverId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            await ValidateStoreOwnershipAsync(order.StoreId, storeOwnerId);

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Hanya pesanan Pending yang bisa diassign driver jemput.");

            if (order.DeliveryType != DeliveryType.Courier)
                throw new Exception("Hanya pesanan Antar-Jemput (Courier) yang bisa diassign driver jemput.");

            var pickupDriver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.Id == pickupDriverId);
            if (pickupDriver == null)
                throw new Exception("Driver tidak ditemukan.");
            if (pickupDriver.StoreId != order.StoreId)
                throw new Exception("Driver tidak terdaftar di toko ini.");
            if (!pickupDriver.IsAvailable)
                throw new Exception("Driver sedang nonaktif dan tidak bisa ditugaskan.");

            int pickupActiveCount = await _context.Orders.CountAsync(o =>
                (o.PickupDriverId == pickupDriverId && o.Status == OrderStatus.OnPickup) ||
                (o.DeliveryDriverId == pickupDriverId && o.Status == OrderStatus.OnDelivery));
            if (pickupActiveCount >= 5)
                throw new Exception("Driver sedang memiliki terlalu banyak tugas aktif.");

            order.PickupDriverId = pickupDriverId;
            order.Status = OrderStatus.OnPickup;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            // Notify Driver
            var driver = await _context.DriverProfiles.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == pickupDriverId);
            if (driver != null && driver.UserId != Guid.Empty)
            {
                await _dispatcher.DispatchNotificationAsync(driver.UserId, "Tugas Jemput Baru!", "Anda ditugaskan untuk menjemput cucian pelanggan.");
            }

            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<OrderResponseDto> ReadyForDeliveryAsync(Guid orderId, Guid storeOwnerId, Guid deliveryDriverId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Pesanan tidak ditemukan.");

            await ValidateStoreOwnershipAsync(order.StoreId, storeOwnerId);

            if (order.Status != OrderStatus.Washing)
                throw new Exception("Pesanan belum siap diantar.");

            if (order.DeliveryType != DeliveryType.Courier)
                throw new Exception("Hanya pesanan Antar-Jemput (Courier) yang bisa diassign driver antar.");

            var deliveryDriver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.Id == deliveryDriverId);
            if (deliveryDriver == null)
                throw new Exception("Driver tidak ditemukan.");
            if (deliveryDriver.StoreId != order.StoreId)
                throw new Exception("Driver tidak terdaftar di toko ini.");
            if (!deliveryDriver.IsAvailable)
                throw new Exception("Driver sedang nonaktif dan tidak bisa ditugaskan.");

            int deliveryActiveCount = await _context.Orders.CountAsync(o =>
                (o.PickupDriverId == deliveryDriverId && o.Status == OrderStatus.OnPickup) ||
                (o.DeliveryDriverId == deliveryDriverId && o.Status == OrderStatus.OnDelivery));
            if (deliveryActiveCount >= 5)
                throw new Exception("Driver sedang memiliki terlalu banyak tugas aktif.");

            order.DeliveryDriverId = deliveryDriverId;
            order.Status = OrderStatus.OnDelivery;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            // Notify Driver
            var driver = await _context.DriverProfiles.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == deliveryDriverId);
            if (driver != null && driver.UserId != Guid.Empty)
            {
                await _dispatcher.DispatchNotificationAsync(driver.UserId, "Tugas Antar Baru!", "Cucian telah selesai dan siap diantar ke pelanggan.");
            }

            return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
        }

        public async Task<OrderResponseDto> ChangePaymentMethodAsync(Guid orderId, Guid customerId, string newMethod)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.CustomerId != customerId) throw new Exception("Pesanan tidak ditemukan.");

            if (order.Status != OrderStatus.AwaitingPayment)
                throw new Exception("Hanya bisa mengubah metode bayar saat menunggu pembayaran.");

            if (Enum.TryParse<PaymentMethod>(newMethod, out var parsedMethod))
            {
                order.PaymentMethod = parsedMethod;
                if (parsedMethod == PaymentMethod.PayLater)
                {
                    order.Status = OrderStatus.Washing; // Langsung dicuci jika PayLater
                }
                var updatedOrder = await _orderRepository.UpdateOrderAsync(order);
                return MapToResponseDto(updatedOrder, order.Store?.Name ?? "Toko");
            }
            
            throw new Exception("Metode pembayaran tidak valid.");
        }
    }
}