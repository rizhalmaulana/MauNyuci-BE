using MauNyuci.Api.Data;
using MauNyuci.Api.Models;
using MauNyuci.Api.DTOs.Order;
using MauNyuci.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogItem)
                .Include(o => o.Store)
                .Include(o => o.Customer)
                .Include(o => o.SelectedStoreBankAccount)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<(IEnumerable<Order> Data, int TotalItems)> GetByCustomerIdAsync(Guid customerId, string? status, int page, int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogItem)
                .Include(o => o.Store)
                .Include(o => o.SelectedStoreBankAccount)
                .Where(o => o.CustomerId == customerId);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(o => o.Status == parsedStatus);
            }

            var totalItems = await query.CountAsync();

            var data = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalItems);
        }

        public async Task<IEnumerable<Order>> GetByStoreIdAsync(Guid storeId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogItem)
                .Include(o => o.Customer)
                .Include(o => o.SelectedStoreBankAccount)
                .Where(o => o.StoreId == storeId && o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Order> Data, int TotalItems)> GetHistoryByStoreIdAsync(Guid storeId, string? search, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogItem)
                .Include(o => o.Customer)
                .Include(o => o.SelectedStoreBankAccount)
                .Where(o => o.StoreId == storeId && (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(o => 
                    (o.IsManualOrder && o.GuestCustomerName != null && o.GuestCustomerName.ToLower().Contains(searchLower)) ||
                    (!o.IsManualOrder && o.Customer != null && o.Customer.FullName.ToLower().Contains(searchLower)) ||
                    (o.IsManualOrder && o.GuestCustomerPhone != null && o.GuestCustomerPhone.ToLower().Contains(searchLower)) ||
                    (!o.IsManualOrder && o.Customer != null && o.Customer.PhoneNumber != null && o.Customer.PhoneNumber.ToLower().Contains(searchLower)) ||
                    o.Id.ToString().ToLower().Contains(searchLower)
                );
            }

            if (startDate.HasValue)
            {
                var start = DateTime.SpecifyKind(startDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(o => o.CreatedAt >= start);
            }

            if (endDate.HasValue)
            {
                var end = DateTime.SpecifyKind(endDate.Value.Date, DateTimeKind.Utc).AddDays(1).AddTicks(-1);
                query = query.Where(o => o.CreatedAt <= end);
            }

            var totalItems = await query.CountAsync();

            var data = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalItems);
        }

        public async Task<StoreTransactionsSummaryDto> GetStoreTransactionsSummaryAsync(Guid storeId, DateTime? startDate, DateTime? endDate)
        {
            var baseQuery = _context.Orders.Where(o => o.StoreId == storeId);

            DateTime start, end;
            if (startDate.HasValue && endDate.HasValue)
            {
                start = startDate.Value.ToUniversalTime();
                end = endDate.Value.ToUniversalTime().AddDays(1).AddTicks(-1);
            }
            else
            {
                // Default: Hari Ini
                start = DateTime.UtcNow.Date;
                end = start.AddDays(1).AddTicks(-1);
            }

            // 1. Query Total Order (Berdasarkan Kapan Dibuat)
            var createdOrders = await baseQuery.Where(o => o.CreatedAt >= start && o.CreatedAt <= end).ToListAsync();
            
            // 2. Query Total Omset (Berdasarkan Kapan Dibayar)
            var paidOrders = await baseQuery.Where(o => o.PaidAt >= start && o.PaidAt <= end && o.PaymentStatus == PaymentStatus.Paid && o.Status != OrderStatus.Cancelled).ToListAsync();

            // 3. Query untuk menghitung Status Aktif (Mengabaikan Tanggal Filter)
            var allOrders = await baseQuery.ToListAsync();

            return new StoreTransactionsSummaryDto
            {
                TotalOrders = createdOrders.Count,
                TotalRevenue = paidOrders.Sum(o => o.TotalAmount),
                
                // Status antrean diambil dari seluruh transaksi (Hutang pekerjaan toko)
                PendingOrders = allOrders.Count(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.AwaitingPayment),
                WashingOrders = allOrders.Count(o => o.Status == OrderStatus.Washing || o.Status == OrderStatus.WaitingForDropOff || o.Status == OrderStatus.OnPickup),
                ReadyForPickupOrders = allOrders.Count(o => o.Status == OrderStatus.ReadyForPickup || o.Status == OrderStatus.OnDelivery),
                CompletedOrders = allOrders.Count(o => o.Status == OrderStatus.Completed),
                CancelledOrders = allOrders.Count(o => o.Status == OrderStatus.Cancelled)
            };
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}