using MauNyuci.Api.DTOs.StoreCatalog;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IStoreCatalogService
    {
        Task<IEnumerable<StoreCatalogResponseDto>> GetMyStoreCatalogAsync(Guid ownerId);
        Task<StoreCatalogResponseDto> AddCatalogItemAsync(Guid ownerId, StoreCatalogCreateDto request);
        Task<IEnumerable<StoreCatalogResponseDto>> GetStoreCatalogsAsync(Guid storeId);
        Task<StoreCatalogResponseDto> UpdateCatalogItemAsync(Guid ownerId, Guid catalogId, StoreCatalogUpdateDto request);
        Task DeleteCatalogItemAsync(Guid ownerId, Guid catalogId);
    }
}