using MauNyuci.Api.DTOs.Store;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IStoreService
    {
        Task<StoreResponseDto> CreateStoreAsync(Guid ownerId, StoreCreateRequestDto request);
        Task<IEnumerable<StoreResponseDto>> GetAllStoresAsync();
        Task<IEnumerable<StoreResponseDto>> GetNearbyStoresAsync(double userLat, double userLng, double radiusInKm);
        Task<StoreResponseDto?> GetMyStoreAsync(Guid userId);
        Task<StoreResponseDto> UpdateStoreAsync(Guid userId, StoreUpdateRequestDto request);
    }
}