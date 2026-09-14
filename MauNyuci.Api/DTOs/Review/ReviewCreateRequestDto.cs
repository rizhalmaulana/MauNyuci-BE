using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.Review
{
    public class ReviewCreateRequestDto
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating harus antara 1 sampai 5.")]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }

    public class ReviewResponseDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}