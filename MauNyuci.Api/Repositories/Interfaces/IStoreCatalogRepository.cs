using MauNyuci.Api.Models;

namespace MauNyuci.Api.Repositories.Interfaces
{
    public interface IStoreCatalogRepository
    {
        Task<StoreCatalogItem> CreateAsync(StoreCatalogItem storeService);
        Task<IEnumerable<StoreCatalogItem>> GetByStoreIdAsync(Guid storeId);
        Task<StoreCatalogItem?> GetByIdAsync(Guid id);
        Task<StoreCatalogItem> UpdateAsync(StoreCatalogItem storeService);
        Task DeleteAsync(StoreCatalogItem storeService);
    }
}