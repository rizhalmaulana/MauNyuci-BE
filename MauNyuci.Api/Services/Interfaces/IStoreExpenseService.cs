using MauNyuci.Api.DTOs.Membership;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IStoreExpenseService
    {
        Task<StoreExpenseResponseDto> CreateExpenseAsync(Guid storeId, StoreExpenseRequestDto request);
        Task<List<StoreExpenseResponseDto>> GetExpensesAsync(Guid storeId, DateTime? startDate, DateTime? endDate);
        Task<StoreExpenseResponseDto> UpdateExpenseAsync(Guid storeId, Guid expenseId, StoreExpenseRequestDto request);
        Task<bool> DeleteExpenseAsync(Guid storeId, Guid expenseId);
    }
}
