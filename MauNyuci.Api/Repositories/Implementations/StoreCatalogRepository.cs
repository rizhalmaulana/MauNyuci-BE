using MauNyuci.Api.Data;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Repositories.Implementations
{
    public class StoreCatalogRepository : IStoreCatalogRepository
    {
        private readonly AppDbContext _context;

        public StoreCatalogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StoreCatalogItem> CreateAsync(StoreCatalogItem storeService)
        {
            await _context.StoreCatalogItems.AddAsync(storeService);
            await _context.SaveChangesAsync();
            return storeService;
        }

        public async Task<IEnumerable<StoreCatalogItem>> GetByStoreIdAsync(Guid storeId)
        {
            return await _context.StoreCatalogItems
                .Where(s => s.StoreId == storeId)
                .OrderBy(s => s.Category).ThenBy(s => s.Price)
                .ToListAsync();
        }

        public async Task<StoreCatalogItem?> GetByIdAsync(Guid id)
        {
            return await _context.StoreCatalogItems.FindAsync(id);
        }

        public async Task<StoreCatalogItem> UpdateAsync(StoreCatalogItem storeService)
        {
            storeService.UpdatedAt = DateTime.UtcNow;
            _context.StoreCatalogItems.Update(storeService);
            await _context.SaveChangesAsync();
            return storeService;
        }

        public async Task DeleteAsync(StoreCatalogItem storeService)
        {
            _context.StoreCatalogItems.Remove(storeService);
            await _context.SaveChangesAsync();
        }
    }
}