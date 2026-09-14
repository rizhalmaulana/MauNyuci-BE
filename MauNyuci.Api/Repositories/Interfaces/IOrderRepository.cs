using MauNyuci.Api.Models;
using MauNyuci.Api.DTOs.Order;

namespace MauNyuci.Api.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetByIdAsync(Guid orderId);
        Task<(IEnumerable<Order> Data, int TotalItems)> GetByCustomerIdAsync(Guid customerId, string? status, int page, int pageSize);
        Task<IEnumerable<Order>> GetByStoreIdAsync(Guid storeId);
        Task<(IEnumerable<Order> Data, int TotalItems)> GetHistoryByStoreIdAsync(Guid storeId, string? search, DateTime? startDate, DateTime? endDate, int page, int pageSize);
        Task<StoreTransactionsSummaryDto> GetStoreTransactionsSummaryAsync(Guid storeId, DateTime? startDate, DateTime? endDate);
        Task<Order> UpdateOrderAsync(Order order);
    }
}