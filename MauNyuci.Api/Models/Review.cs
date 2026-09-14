using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.Models
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid StoreId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // Hanya boleh 1 sampai 5

        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Order? Order { get; set; }
        public Store? Store { get; set; }
        public User? Customer { get; set; }
    }
}