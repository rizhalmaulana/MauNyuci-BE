using MauNyuci.Api.DTOs.StoreCatalog;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using MauNyuci.Api.Services.Interfaces;

namespace MauNyuci.Api.Services.Implementations
{
    public class StoreCatalogService : IStoreCatalogService
    {
        private readonly IStoreCatalogRepository _catalogRepository;
        private readonly IStoreRepository _storeRepository;

        public StoreCatalogService(IStoreCatalogRepository catalogRepository, IStoreRepository storeRepository)
        {
            _catalogRepository = catalogRepository;
            _storeRepository = storeRepository;
        }

        public async Task<IEnumerable<StoreCatalogResponseDto>> GetMyStoreCatalogAsync(Guid ownerId)
        {
            var store = await _storeRepository.GetByOwnerOrStaffIdAsync(ownerId);
            if (store == null)
            {
                throw new Exception("Anda belum mendaftarkan toko atau bukan staff aktif.");
            }

            var catalogs = await _catalogRepository.GetByStoreIdAsync(store.Id);
            return catalogs.Select(MapToDto);
        }

        public async Task<StoreCatalogResponseDto> AddCatalogItemAsync(Guid ownerId, StoreCatalogCreateDto request)
        {
            var store = await _storeRepository.GetByOwnerOrStaffIdAsync(ownerId);
            if (store == null)
            {
                throw new Exception("Anda belum mendaftarkan toko atau bukan staff aktif.");
            }

            if (request.StoreId.HasValue && request.StoreId.Value != store.Id)
            {
                throw new Exception("Anda tidak memiliki akses untuk menambah layanan ke toko ini.");
            }

            var newService = new MauNyuci.Api.Models.StoreCatalogItem
            {
                StoreId = store.Id,
                Category = request.Category,
                ServiceType = request.ServiceType,
                Name = request.Name,
                Description = request.Description,
                ImageAsset = request.ImageAsset,
                Price = request.Price,
                Unit = request.Unit,
                TimeEstimate = request.TimeEstimate,
                IsAvailable = true
            };

            var createdService = await _catalogRepository.CreateAsync(newService);

            return MapToDto(createdService);
        }

        public async Task<StoreCatalogResponseDto> UpdateCatalogItemAsync(Guid ownerId, Guid catalogId, StoreCatalogUpdateDto request)
        {
            var store = await _storeRepository.GetByOwnerOrStaffIdAsync(ownerId);
            if (store == null)
            {
                throw new Exception("Anda belum mendaftarkan toko atau bukan staff aktif.");
            }

            var catalogItem = await _catalogRepository.GetByIdAsync(catalogId);
            if (catalogItem == null || catalogItem.StoreId != store.Id)
            {
                throw new Exception("Layanan tidak ditemukan atau Anda tidak memiliki akses.");
            }

            if (request.Category != null) catalogItem.Category = request.Category;
            if (request.ServiceType != null) catalogItem.ServiceType = request.ServiceType;
            if (request.Name != null) catalogItem.Name = request.Name;
            if (request.Description != null) catalogItem.Description = request.Description;
            if (request.ImageAsset != null) catalogItem.ImageAsset = request.ImageAsset;
            if (request.Price.HasValue) catalogItem.Price = request.Price.Value;
            if (request.Unit != null) catalogItem.Unit = request.Unit;
            if (request.TimeEstimate != null) catalogItem.TimeEstimate = request.TimeEstimate;
            if (request.IsAvailable.HasValue) catalogItem.IsAvailable = request.IsAvailable.Value;

            var updatedService = await _catalogRepository.UpdateAsync(catalogItem);

            return MapToDto(updatedService);
        }

        public async Task DeleteCatalogItemAsync(Guid ownerId, Guid catalogId)
        {
            var store = await _storeRepository.GetByOwnerOrStaffIdAsync(ownerId);
            if (store == null)
            {
                throw new Exception("Anda belum mendaftarkan toko atau bukan staff aktif.");
            }

            var catalogItem = await _catalogRepository.GetByIdAsync(catalogId);
            if (catalogItem == null || catalogItem.StoreId != store.Id)
            {
                throw new Exception("Layanan tidak ditemukan atau Anda tidak memiliki akses.");
            }

            await _catalogRepository.DeleteAsync(catalogItem);
        }

        public async Task<IEnumerable<StoreCatalogResponseDto>> GetStoreCatalogsAsync(Guid storeId)
        {
            var catalogs = await _catalogRepository.GetByStoreIdAsync(storeId);
            return catalogs.Select(MapToDto);
        }

        private StoreCatalogResponseDto MapToDto(MauNyuci.Api.Models.StoreCatalogItem service)
        {
            return new StoreCatalogResponseDto
            {
                Id = service.Id,
                StoreId = service.StoreId,
                Category = service.Category,
                ServiceType = service.ServiceType,
                Name = service.Name,
                Description = service.Description,
                ImageAsset = service.ImageAsset,
                Price = service.Price,
                Unit = service.Unit,
                IsAvailable = service.IsAvailable,
                TimeEstimate = service.TimeEstimate
            };
        }
    }
}