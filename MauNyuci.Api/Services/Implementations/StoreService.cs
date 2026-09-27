using MauNyuci.Api.DTOs.Store;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using MauNyuci.Api.Services.Interfaces;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MauNyuci.Api.Services.Implementations
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IMediaService _mediaService;

        public StoreService(IStoreRepository storeRepository, IMediaService mediaService)
        {
            _storeRepository = storeRepository;
            _mediaService = mediaService;
        }

        public async Task<StoreResponseDto> CreateStoreAsync(Guid ownerId, StoreCreateRequestDto request)
        {
            // 1. Siapkan mesin pembuat Geometri dengan standar GPS (SRID 4326)
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            Point? storeLocation = null;

            // 2. Jika Front-end mengirimkan Lat Lng, ubah menjadi tipe Point
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                // INGAT: Urutannya adalah (X, Y) -> (Longitude, Latitude)
                storeLocation = geometryFactory.CreatePoint(new Coordinate(request.Longitude.Value, request.Latitude.Value));
            }

            var newStore = new Store
            {
                OwnerId = ownerId,
                Name = request.Name,
                Address = request.Address,
                Location = storeLocation,
                StorePhoneNumber = request.StorePhoneNumber,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                HasPickupDeliveryService = request.HasPickupDeliveryService,
                PickupDeliveryFee = request.PickupDeliveryFee,
                MinOrderForPickup = request.MinOrderForPickup
            };

            if (request.ImageFile != null)
            {
                newStore.StoreImageUrl = await _mediaService.UploadImageAsync(request.ImageFile, "stores");
            }

            var createdStore = await _storeRepository.CreateAsync(newStore);
            return MapToDto(createdStore);
        }

        public async Task<IEnumerable<StoreResponseDto>> GetAllStoresAsync()
        {
            var stores = await _storeRepository.GetAllAsync();
            return stores.Select(MapToDto);
        }

        public async Task<StoreResponseDto?> GetStoreByIdAsync(Guid storeId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null) return null;
            return MapToDto(store);
        }

        public async Task<IEnumerable<StoreResponseDto>> GetNearbyStoresAsync(double userLat, double userLng, double radiusInKm)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            // Ingat urutannya: Longitude (X), Latitude (Y)
            var userLocation = geometryFactory.CreatePoint(new Coordinate(userLng, userLat));

            var stores = await _storeRepository.GetNearbyAsync(userLocation, radiusInKm);

            // Kita map ke DTO sekalian menghitung jaraknya
            return stores.Select(store =>
            {
                var dto = MapToDto(store);
                // Hitung ulang jaraknya untuk ditampilkan ke user (Derajat dikali 111 untuk jadi KM)
                if (store.Location != null)
                {
                    dto.DistanceInKm = Math.Round(store.Location.Distance(userLocation) * 111.0, 2);
                }
                return dto;
            });
        }

        public async Task<StoreResponseDto?> GetMyStoreAsync(Guid userId)
        {
            var store = await _storeRepository.GetByOwnerOrStaffIdAsync(userId);

            // Jika user ini belum punya toko, kembalikan null
            if (store == null) return null;

            return MapToDto(store);
        }

        public async Task<StoreResponseDto> UpdateStoreAsync(Guid userId, StoreUpdateRequestDto request)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(userId);
            if (store == null)
            {
                throw new Exception("Toko tidak ditemukan.");
            }

            if (request.Name != null) store.Name = request.Name;
            if (request.Address != null) store.Address = request.Address;
            if (request.StorePhoneNumber != null) store.StorePhoneNumber = request.StorePhoneNumber;
            
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                store.Location = geometryFactory.CreatePoint(new Coordinate(request.Longitude.Value, request.Latitude.Value));
            }

            if (request.OpenTime.HasValue) store.OpenTime = request.OpenTime.Value;
            if (request.CloseTime.HasValue) store.CloseTime = request.CloseTime.Value;
            if (request.IsOpen.HasValue) store.IsOpen = request.IsOpen.Value;
            if (request.HasPickupDeliveryService.HasValue) store.HasPickupDeliveryService = request.HasPickupDeliveryService.Value;
            if (request.PickupDeliveryFee.HasValue) store.PickupDeliveryFee = request.PickupDeliveryFee.Value;
            if (request.MinOrderForPickup.HasValue) store.MinOrderForPickup = request.MinOrderForPickup.Value;

            if (request.ImageFile != null)
            {
                store.StoreImageUrl = await _mediaService.UploadImageAsync(request.ImageFile, "stores");
            }

            store.UpdatedAt = DateTime.UtcNow;

            var updatedStore = await _storeRepository.UpdateAsync(store);
            return MapToDto(updatedStore);
        }

        // Fungsi bantuan untuk mengubah Model menjadi DTO
        private StoreResponseDto MapToDto(Store store)
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var isTimeOpen = store.OpenTime <= currentTime && store.CloseTime >= currentTime;

            return new StoreResponseDto
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Latitude = store.Location?.Y,
                Longitude = store.Location?.X,
                OperatingHoursFormatted = $"{store.OpenTime:hh\\:mm} - {store.CloseTime:hh\\:mm}",
                IsCurrentlyOpen = store.IsOpen && isTimeOpen,
                AverageRating = store.AverageRating,
                TotalReviews = store.TotalReviews,
                StoreImageUrl = store.StoreImageUrl,
                StorePhoneNumber = store.StorePhoneNumber,
                HasPickupDeliveryService = store.HasPickupDeliveryService,
                PickupDeliveryFee = store.PickupDeliveryFee,
                MinOrderForPickup = store.MinOrderForPickup
            };
        }
    }
}