using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    public class DriverSettlement
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid DriverId { get; set; }
        [ForeignKey("DriverId")]
        public DriverProfile? Driver { get; set; }

        [Required]
        public Guid StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Berapa yang disetor

        public DateTime SettledAt { get; set; } = DateTime.UtcNow;
        public Guid VerifiedByAdminId { get; set; } // Siapa kasir yang terima uangnya
    }
}