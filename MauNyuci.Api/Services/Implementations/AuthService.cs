using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs.Auth;
using MauNyuci.Api.Models;
using MauNyuci.Api.Services.Interfaces;
using FirebaseAdmin.Auth;
using MauNyuci.Api.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MauNyuci.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMediaService _mediaService;

        public AuthService(AppDbContext context, IConfiguration config, IMediaService mediaService)
        {
            _context = context;
            _config = config;
            _mediaService = mediaService;
        }

        // Logika Register Manual
        public async Task<AuthResponseDto?> RegisterLocalAsync(RegisterRequestDto request)
        {
            // Cek apakah nomor HP sudah terdaftar
            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (existingUser != null) return null; // Gagal, user sudah ada

            // Cek email uniqueness jika email diisi
            if (!string.IsNullOrEmpty(request.Email))
            {
                var existingEmail = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingEmail != null) return null;
            }

            var user = new User
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email,
                AuthProvider = "Local",
                Role = "Customer",
                MembershipTierId = MembershipTierConstants.RegularId
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();
            
            // Reload user with TierInfo for response
            user = await _context.User.Include(u => u.TierInfo).FirstAsync(u => u.Id == user.Id);

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                FullName = user.FullName,
                Role = user.Role,
                StoreRole = null,
                MembershipTier = user.TierInfo?.Name ?? MembershipTierConstants.Regular,
                IsProfileComplete = true
            };
        }

        // Logika Login Manual
        public async Task<AuthResponseDto?> LoginLocalAsync(LoginRequestDto request)
        {
            var user = await _context.User
                .Include(u => u.TierInfo)
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

            // Cek keberadaan user dan validitas password
            if (user == null || user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null; // Login gagal
            }

            if (!user.IsActive)
            {
                throw new Exception("Akun Anda telah dinonaktifkan. Silakan hubungi admin.");
            }

            // Validasi AppType
            if (request.AppType == "Customer" && user.Role != "Customer")
            {
                throw new Exception("Akun ini terdaftar sebagai Karyawan/Pemilik Toko atau Driver. Tidak dapat digunakan pada aplikasi Customer.");
            }
            if (request.AppType == "Store" && user.Role != "Owner" && user.Role != "StoreStaff")
            {
                throw new Exception("Akun ini terdaftar sebagai Customer. Tidak dapat login di aplikasi Toko.");
            }
            StoreStaff? staff = null;
            if (user.Role == "StoreStaff")
            {
                staff = await _context.StoreStaffs.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (staff != null && !staff.IsActive)
                {
                    throw new Exception("Akun Staff Anda dinonaktifkan dari Toko.");
                }
            }

            if (request.AppType == "Driver")
            {
                bool isDriver = user.Role == "Driver" || (staff != null && staff.Role == "Driver");
                if (!isDriver)
                {
                    throw new Exception("Akun Anda bukan akun Driver.");
                }
            }

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user, staff),
                FullName = user.FullName,
                Role = user.Role,
                StoreRole = user.Role == "Owner" ? "Owner" : staff?.Role,
                MembershipTier = user.TierInfo?.Name ?? MembershipTierConstants.Regular,
                IsProfileComplete = !string.IsNullOrEmpty(user.PhoneNumber)
            };
        }

        // Data Profil
        public async Task<UserProfileResponseDto?> GetUserProfileAsync(Guid userId)
        {
            var user = await _context.User
                .Include(u => u.TierInfo)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            string? storeRole = null;
            if (user.Role == "Owner") storeRole = "Owner";
            else if (user.Role == "StoreStaff")
            {
                var staff = await _context.StoreStaffs.FirstOrDefaultAsync(s => s.UserId == user.Id);
                storeRole = staff?.Role;
            }

            return new UserProfileResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                DefaultAddress = user.DefaultAddress,
                DefaultLatitude = user.DefaultLatitude,
                DefaultLongitude = user.DefaultLongitude,
                Role = user.Role,
                StoreRole = storeRole,
                AuthProvider = user.AuthProvider,
                MembershipTier = user.TierInfo?.Name ?? MembershipTierConstants.Regular
            };
        }

        public async Task<UserProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("Pengguna tidak ditemukan.");

            // Cek uniqueness phone number jika diisi
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                var existingPhone = await _context.User.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId);
                if (existingPhone != null) throw new Exception("Nomor telepon sudah digunakan oleh user lain.");
            }

            // Update field sesuai permintaan baru (Partial Update)
            user.FullName = request.FullName; // FullName wajib diisi
            
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
            
            if (request.Email != null) 
                user.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email;
            
            // Handle foto profil dari URL
            if (request.ProfilePictureUrl != null)
            {
                user.ProfilePictureUrl = request.ProfilePictureUrl;
            }

            if (request.DefaultAddress != null) user.DefaultAddress = request.DefaultAddress;
            if (request.DefaultLatitude != null) user.DefaultLatitude = request.DefaultLatitude;
            if (request.DefaultLongitude != null) user.DefaultLongitude = request.DefaultLongitude;

            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return (await GetUserProfileAsync(userId))!;
        }

        // Generator Tiket JWT
        private string GenerateJwtToken(User user, StoreStaff? staff = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("MembershipTier", user.TierInfo?.Name ?? MembershipTierConstants.Regular)
            };

            if (staff != null)
            {
                claims.Add(new Claim("StoreId", staff.StoreId.ToString()));
                claims.Add(new Claim("StoreRole", staff.Role));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Token berlaku 7 hari
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<CheckUserExistsResponseDto> CheckUserExistsAsync(CheckUserExistsRequestDto request)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => 
                (request.PhoneNumber != null && u.PhoneNumber == request.PhoneNumber) ||
                (request.Email != null && u.Email == request.Email));

            if (user == null)
            {
                return new CheckUserExistsResponseDto
                {
                    Exists = false,
                    Message = "User tidak ditemukan"
                };
            }

            return new CheckUserExistsResponseDto
            {
                Exists = true,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Message = "User sudah terdaftar"
            };
        }

        public async Task<AuthResponseDto?> AuthenticateWithFirebaseAsync(FirebaseAuthRequestDto request)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
                var email = decodedToken.Claims["email"]?.ToString();
                var name = decodedToken.Claims["name"]?.ToString();
                var picture = decodedToken.Claims["picture"]?.ToString();

                if (string.IsNullOrEmpty(email))
                    return null;

                var existingUser = await _context.User
                    .Include(u => u.TierInfo)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (existingUser != null)
                {
                    if (!existingUser.IsActive)
                        throw new Exception("Akun Anda telah dinonaktifkan. Silakan hubungi admin.");

                    // Validasi AppType
                    if (request.AppType == "Customer" && existingUser.Role != "Customer")
                    {
                        throw new Exception("Akun ini terdaftar sebagai Karyawan/Pemilik Toko atau Driver. Tidak dapat digunakan pada aplikasi Customer.");
                    }
                    if (request.AppType == "Store" && existingUser.Role != "Owner" && existingUser.Role != "StoreStaff")
                    {
                        throw new Exception("Akun ini terdaftar sebagai Customer. Tidak dapat login di aplikasi Toko.");
                    }
                    StoreStaff? staff = null;
                    if (existingUser.Role == "StoreStaff")
                    {
                        staff = await _context.StoreStaffs.FirstOrDefaultAsync(s => s.UserId == existingUser.Id);
                    }

                    if (request.AppType == "Driver")
                    {
                        bool isDriver = existingUser.Role == "Driver" || (staff != null && staff.Role == "Driver");
                        if (!isDriver)
                        {
                            throw new Exception("Akun Anda bukan akun Driver.");
                        }
                    }

                    return new AuthResponseDto
                    {
                        Token = GenerateJwtToken(existingUser, staff),
                        FullName = existingUser.FullName,
                        Role = existingUser.Role,
                        StoreRole = existingUser.Role == "Owner" ? "Owner" : staff?.Role,
                        MembershipTier = existingUser.TierInfo?.Name ?? MembershipTierConstants.Regular,
                        IsProfileComplete = !string.IsNullOrEmpty(existingUser.PhoneNumber)
                    };
                }

                if (request.AppType != "Customer")
                {
                    throw new Exception("Akun tidak ditemukan. Pendaftaran otomatis (Register) hanya dapat dilakukan melalui aplikasi Customer.");
                }

                var newUser = new User
                {
                    FullName = string.IsNullOrEmpty(request.FullName) ? name ?? "User" : request.FullName,
                    Email = email,
                    PhoneNumber = request.PhoneNumber,
                    ProfilePictureUrl = picture,
                    AuthProvider = "Firebase",
                    Role = "Customer",
                    MembershipTierId = MembershipTierConstants.RegularId
                };

                _context.User.Add(newUser);
                await _context.SaveChangesAsync();
                
                // Reload user to get TierInfo
                newUser = await _context.User.Include(u => u.TierInfo).FirstAsync(u => u.Id == newUser.Id);

                return new AuthResponseDto
                {
                    Token = GenerateJwtToken(newUser),
                    FullName = newUser.FullName,
                    Role = newUser.Role,
                    StoreRole = null,
                    MembershipTier = newUser.TierInfo?.Name ?? MembershipTierConstants.Regular,
                    IsProfileComplete = !string.IsNullOrEmpty(newUser.PhoneNumber)
                };
            }
            catch (Exception ex)
            {
                if (ex.Message == "Akun Anda telah dinonaktifkan. Silakan hubungi admin." || 
                    ex.Message.Contains("Tidak dapat digunakan pada aplikasi") ||
                    ex.Message.Contains("Tidak dapat login di aplikasi") ||
                    ex.Message.Contains("bukan akun Driver") ||
                    ex.Message.Contains("Pendaftaran otomatis"))
                {
                    throw; // Teruskan exception agar Controller mengembalikan BadRequest dengan message ini
                }

                return null;
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("Pengguna tidak ditemukan.");

            if (user.AuthProvider != "Local")
                throw new Exception("Pengguna dengan metode login pihak ketiga tidak memiliki password lokal.");

            if (user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                throw new Exception("Password lama tidak sesuai.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            
            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}