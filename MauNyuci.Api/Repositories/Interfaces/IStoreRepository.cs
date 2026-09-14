using MauNyuci.Api.Models;
using NetTopologySuite.Geometries;

namespace MauNyuci.Api.Repositories.Interfaces
{
    public interface IStoreRepository
    {
        Task<Store?> GetByIdAsync(Guid id);
        Task<Store> CreateAsync(Store store);
        Task<IEnumerable<Store>> GetAllAsync();

        // Tambahkan parameter userLocation
        Task<IEnumerable<Store>> GetNearbyAsync(Point userLocation, double maxDistanceMeter);

        Task<Store?> GetByOwnerIdAsync(Guid ownerId);
        Task<Store?> GetByOwnerOrStaffIdAsync(Guid userId);
        Task<Store> UpdateAsync(Store store);
    }
}