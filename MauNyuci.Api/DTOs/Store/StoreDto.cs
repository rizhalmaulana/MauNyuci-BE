using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MauNyuci.Api.DTOs.Store
{
    public class StoreCreateRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? StorePhoneNumber { get; set; }

        public TimeSpan OpenTime { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan CloseTime { get; set; } = new TimeSpan(20, 0, 0);

        public bool HasPickupDeliveryService { get; set; }
        public decimal PickupDeliveryFee { get; set; }
        public decimal MinOrderForPickup { get; set; }

        public IFormFile? ImageFile { get; set; }
    }

    public class StoreUpdateRequestDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? StorePhoneNumber { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public bool? IsOpen { get; set; }
        public bool? HasPickupDeliveryService { get; set; }
        public decimal? PickupDeliveryFee { get; set; }
        public decimal? MinOrderForPickup { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class StoreResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? DistanceInKm { get; set; }

        public string OperatingHoursFormatted { get; set; } = string.Empty;
        public bool IsCurrentlyOpen { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public string? StoreImageUrl { get; set; }

        public string? StorePhoneNumber { get; set; }
        public bool HasPickupDeliveryService { get; set; }
        public decimal PickupDeliveryFee { get; set; }
        public decimal MinOrderForPickup { get; set; }
    }
}