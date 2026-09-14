using MauNyuci.Api.DTOs.Inventory;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryItemResponseDto> CreateInventoryItemAsync(Guid storeId, Guid userId, InventoryItemCreateDto request);
        Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsAsync(Guid storeId, Guid userId);
        Task<InventoryItemResponseDto> UpdateInventoryItemAsync(Guid storeId, Guid userId, Guid itemId, InventoryItemUpdateDto request);
        Task RemoveInventoryItemAsync(Guid storeId, Guid userId, Guid itemId);
        
        Task<InventoryTransactionResponseDto> AddTransactionAsync(Guid storeId, Guid userId, Guid itemId, InventoryTransactionCreateDto request);
        Task<IEnumerable<InventoryTransactionResponseDto>> GetItemTransactionsAsync(Guid storeId, Guid userId, Guid itemId);
    }
}
