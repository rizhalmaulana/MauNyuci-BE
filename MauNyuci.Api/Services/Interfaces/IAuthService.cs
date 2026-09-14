using MauNyuci.Api.DTOs.Auth;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterLocalAsync(RegisterRequestDto request);
        Task<AuthResponseDto?> LoginLocalAsync(LoginRequestDto request);
        Task<UserProfileResponseDto?> GetUserProfileAsync(Guid userId);
        Task<UserProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request);
        Task<CheckUserExistsResponseDto> CheckUserExistsAsync(CheckUserExistsRequestDto request);
        Task<AuthResponseDto?> AuthenticateWithFirebaseAsync(FirebaseAuthRequestDto request);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request);
    }
}