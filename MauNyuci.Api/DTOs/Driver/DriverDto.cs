using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.Driver
{
    public class UpdateLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class UpdateDriverProfileDto
    {
        [MaxLength(20)]
        public string? VehicleNumber { get; set; }

        [MaxLength(50)]
        public string? VehicleType { get; set; }

        public bool? IsAvailable { get; set; }
    }

    public class DriverTaskResponseDto
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty; // "Pickup" atau "Delivery"
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public double DistanceInKm { get; set; }
    }

    public class DriverTaskDetailResponseDto
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerPhotoUrl { get; set; }
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? CourierNotes { get; set; }
        public string? CustomerLaundryImageUrl { get; set; }
        public string TaskType { get; set; } = string.Empty; // "Pickup" atau "Delivery"
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? PickupTimeSlot { get; set; }
        public string? DeliveryTimeSlot { get; set; }
        public double DistanceInKm { get; set; }
    }

    public class StoreDriverResponseDto
    {
        public Guid DriverId { get; set; } // DriverProfile.Id -> kirim ini sebagai DriverId saat assign
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int ActiveTaskCount { get; set; }
    }
    public class DriverProfileResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? VehiclePlateNumber { get; set; }
        public string? VehicleType { get; set; }
        public bool IsAvailable { get; set; }
        public decimal UnsettledCash { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class UpdateDriverStatusRequestDto
    {
        public bool IsAvailable { get; set; }
    }

    public class DriverTaskHistorySummaryDto
    {
        public int TotalTasks { get; set; }
        public int TotalPickup { get; set; }
        public int TotalDelivery { get; set; }
        public decimal TotalCashCollected { get; set; }
    }

    public class DriverTaskHistoryItemDto
    {
        public Guid TaskId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class DriverTaskHistoryResponseDto
    {
        public DriverTaskHistorySummaryDto Summary { get; set; } = new();
        public List<DriverTaskHistoryItemDto> Tasks { get; set; } = new();
    }
}