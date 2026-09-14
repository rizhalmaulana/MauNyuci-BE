using MauNyuci.Api.DTOs.Auth;
using MauNyuci.Api.Services.Implementations;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; 

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterLocalAsync(request);
            if (result == null) return BadRequest("Nomor Telepon sudah terdaftar!");

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginLocalAsync(request);
                if (result == null) return Unauthorized(new { message = "Nomor Telepon atau Password salah!" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                // Ambil ID User dari token JWT yang dikirim dari HP
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim == null)
                    return Unauthorized(new { message = "Token tidak valid." });

                var userId = Guid.Parse(userIdClaim);
                var profile = await _authService.GetUserProfileAsync(userId);

                if (profile == null)
                    return NotFound(new { message = "Pengguna tidak ditemukan." });

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
        {
            try
            {
                // Ambil ID dari token JWT
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

                var result = await _authService.UpdateProfileAsync(userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                await _authService.ChangePasswordAsync(userId, request);
                return Ok(new { message = "Password berhasil diubah." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("check-exists")]
        public async Task<IActionResult> CheckUserExists([FromBody] CheckUserExistsRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber) && string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Nomor telepon atau email wajib diisi." });

            var result = await _authService.CheckUserExistsAsync(request);
            return Ok(result);
        }

        [HttpPost("firebase-auth")]
        public async Task<IActionResult> FirebaseAuth([FromBody] FirebaseAuthRequestDto request)
        {
            try
            {
                var result = await _authService.AuthenticateWithFirebaseAsync(request);
                if (result == null) return BadRequest(new { message = "Autentikasi Firebase gagal. Token tidak valid." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
