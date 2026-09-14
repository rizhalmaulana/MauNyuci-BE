using System.ComponentModel.DataAnnotations;

namespace MauNyuci.Api.DTOs.Order
{
    // Dipakai Kasir untuk setuju/tolak bukti transfer
    public class VerifyPaymentRequestDto
    {
        [Required]
        public bool IsApproved { get; set; }

        public string? RejectionReason { get; set; } // Wajib diisi jika IsApproved = false
    }
}