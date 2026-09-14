using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.Store
{
    public class StoreBankAccountCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string BankName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string AccountHolderName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? QrisImageUrl { get; set; }
    }

    public class StoreBankAccountUpdateDto
    {
        [MaxLength(50)]
        public string? BankName { get; set; }

        [MaxLength(50)]
        public string? AccountNumber { get; set; }

        [MaxLength(100)]
        public string? AccountHolderName { get; set; }
        
        public bool? IsActive { get; set; }

        [MaxLength(255)]
        public string? QrisImageUrl { get; set; }
    }

    public class StoreBankAccountResponseDto
    {
        public Guid Id { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? QrisImageUrl { get; set; }
    }
}
