using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    public class StoreBankAccount
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        [Required]
        [MaxLength(50)]
        public string BankName { get; set; } = string.Empty; // Contoh: "BCA", "Mandiri", "GoPay", "OVO"

        [Required]
        [MaxLength(50)]
        public string AccountNumber { get; set; } = string.Empty; // Nomor rekening / No HP

        [Required]
        [MaxLength(100)]
        public string AccountHolderName { get; set; } = string.Empty; // Atas nama siapa

        public bool IsActive { get; set; } = true; // Jika sewaktu-waktu rekeningnya tidak dipakai lagi

        [MaxLength(255)]
        public string? QrisImageUrl { get; set; }
    }
}