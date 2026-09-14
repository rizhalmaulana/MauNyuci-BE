using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.StoreStaff
{
    public class StoreStaffCreateDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Cashier";
    }
}
