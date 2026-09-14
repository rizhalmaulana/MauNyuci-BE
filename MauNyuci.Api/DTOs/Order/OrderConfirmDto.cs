using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.Order
{
    public class OrderConfirmRequestDto
    {
        [Required]
        public List<OrderItemWeightDto> Items { get; set; } = new List<OrderItemWeightDto>();
    }

    public class OrderItemWeightDto
    {
        [Required]
        public Guid OrderItemId { get; set; }

        [Required]
        [Range(0.1, 999, ErrorMessage = "Berat/Jumlah harus lebih dari 0")]
        public decimal ActualQuantity { get; set; } // Berat asli dari timbangan toko
    }
}