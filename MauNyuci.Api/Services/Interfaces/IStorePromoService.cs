using MauNyuci.Api.DTOs.Membership;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IStorePromoService
    {
        Task<StorePromoResponseDto> CreatePromoAsync(Guid storeId, StorePromoRequestDto request);
        Task<List<StorePromoResponseDto>> GetPromosAsync(Guid storeId);
        Task<StorePromoResponseDto> UpdatePromoAsync(Guid storeId, Guid promoId, StorePromoRequestDto request);
        Task<bool> DeletePromoAsync(Guid storeId, Guid promoId);
        Task<StorePromoResponseDto?> ValidatePromoAsync(Guid storeId, string promoCode, decimal orderAmount);
    }
}
