using MauNyuci.Api.Data;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace MauNyuci.Api.Repositories.Implementations
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppDbContext _context;

        public StoreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Store?> GetByIdAsync(Guid id)
        {
            return await _context.Store
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Store> CreateAsync(Store store)
        {
            await _context.Store.AddAsync(store);
            await _context.SaveChangesAsync();
            return store;
        }

        public async Task<IEnumerable<Store>> GetAllAsync()
        {
            return await _context.Store.ToListAsync();
        }

        public async Task<IEnumerable<Store>> GetNearbyAsync(Point userLocation, double radiusInKm)
        {
            // Karena PostGIS SRID 4326 menggunakan satuan "Derajat", 
            // kita konversi secara kasar: 1 derajat = ~111 KM.
            double radiusInDegrees = radiusInKm / 111.0;

            return await _context.Store
                .Where(s => s.Location != null && s.Location.IsWithinDistance(userLocation, radiusInDegrees))
                .OrderBy(s => s.Location!.Distance(userLocation)) // Urutkan dari yang terdekat
                .ToListAsync();
        }

        public async Task<Store?> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.Store
                .FirstOrDefaultAsync(s => s.OwnerId == ownerId);
        }

        public async Task<Store?> GetByOwnerOrStaffIdAsync(Guid userId)
        {
            // Cek apakah user adalah owner
            var store = await _context.Store.FirstOrDefaultAsync(s => s.OwnerId == userId);
            if (store != null) return store;

            // Jika bukan owner, cek apakah user adalah staff aktif
            var staff = await _context.StoreStaffs
                .Include(st => st.Store)
                .FirstOrDefaultAsync(st => st.UserId == userId && st.IsActive);
            
            return staff?.Store;
        }

        public async Task<Store> UpdateAsync(Store store)
        {
            _context.Store.Update(store);
            await _context.SaveChangesAsync();
            return store;
        }
    }
}