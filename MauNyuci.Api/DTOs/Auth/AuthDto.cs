using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace MauNyuci.Api.DTOs.Auth
{
    public class RegisterRequestDto : IValidatableObject
    {
        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Nomor telepon wajib diisi")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; } = string.Empty;

        public string? Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                var digits = Regex.Replace(PhoneNumber, @"\D", "");
                if (digits.Length < 10 || digits.Length > 13)
                {
                    yield return new ValidationResult("Nomor telepon harus 10-13 digit", new[] { nameof(PhoneNumber) });
                }
            }

            if (!string.IsNullOrWhiteSpace(Email) && !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                yield return new ValidationResult("Email tidak valid", new[] { nameof(Email) });
            }
        }
    }

    public class FirebaseAuthRequestDto
    {
        [Required(ErrorMessage = "Token Firebase wajib diisi")]
        public string IdToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipe Aplikasi (AppType) wajib diisi")]
        public string AppType { get; set; } = string.Empty;

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Nomor telepon wajib diisi")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipe Aplikasi (AppType) wajib diisi")]
        public string AppType { get; set; } = string.Empty;
    }

    public class UserProfileResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? DefaultAddress { get; set; }
        public double? DefaultLatitude { get; set; }
        public double? DefaultLongitude { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? StoreRole { get; set; }
        public string AuthProvider { get; set; } = string.Empty;
        public string MembershipTier { get; set; } = string.Empty;
    }

    public class UpdateProfileRequestDto : IValidatableObject
    {
        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }


        public string? ProfilePictureUrl { get; set; }
        public string? DefaultAddress { get; set; }
        public double? DefaultLatitude { get; set; }
        public double? DefaultLongitude { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(Email) && !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                yield return new ValidationResult("Email tidak valid", new[] { nameof(Email) });
            }
        }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? StoreRole { get; set; }
        public string MembershipTier { get; set; } = string.Empty;
        public bool IsProfileComplete { get; set; }
    }

    public class CheckUserExistsRequestDto
    {
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class CheckUserExistsResponseDto
    {
        public bool Exists { get; set; }
        public Guid? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Message { get; set; }
    }

    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "Password lama wajib diisi")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password baru wajib diisi")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
