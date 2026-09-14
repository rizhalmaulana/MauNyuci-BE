using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs.StoreStaff;
using MauNyuci.Api.Models;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Services.Implementations
{
    public class StoreStaffService : IStoreStaffService
    {
        private readonly AppDbContext _context;

        public StoreStaffService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StoreStaffResponseDto> AddStaffAsync(Guid storeId, Guid ownerId, StoreStaffCreateDto request)
        {
            var store = await _context.Store
                .FirstOrDefaultAsync(s => s.Id == storeId && s.OwnerId == ownerId);

            if (store == null)
            {
                throw new Exception("Toko tidak ditemukan atau Anda bukan pemilik toko ini.");
            }

            // Check if phone number already exists
            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (existingUser != null)
            {
                throw new Exception("Nomor Telepon sudah terdaftar.");
            }

            var user = new User
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Role = "StoreStaff",
                AuthProvider = "Local"
            };
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var staff = new StoreStaff
            {
                StoreId = storeId,
                UserId = user.Id,
                Role = request.Role,
                IsActive = true,
                JoinedAt = DateTime.UtcNow
            };

            _context.StoreStaffs.Add(staff);
            await _context.SaveChangesAsync();

            return new StoreStaffResponseDto
            {
                Id = staff.Id,
                StoreId = staff.StoreId,
                UserId = staff.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = staff.Role,
                IsActive = staff.IsActive,
                JoinedAt = staff.JoinedAt
            };
        }

        public async Task<IEnumerable<StoreStaffResponseDto>> GetStoreStaffsAsync(Guid storeId, Guid ownerId)
        {
            var store = await _context.Store
                .FirstOrDefaultAsync(s => s.Id == storeId && s.OwnerId == ownerId);

            if (store == null)
            {
                throw new Exception("Toko tidak ditemukan atau Anda bukan pemilik toko ini.");
            }

            var staffs = await _context.StoreStaffs
                .Include(s => s.User)
                .Where(s => s.StoreId == storeId)
                .ToListAsync();

            return staffs.Select(s => new StoreStaffResponseDto
            {
                Id = s.Id,
                StoreId = s.StoreId,
                UserId = s.UserId,
                FullName = s.User?.FullName ?? "Nama Kosong",
                PhoneNumber = s.User?.PhoneNumber ?? "No Handphone Kosong",
                Email = s.User?.Email ?? "Email Kosong",
                Role = s.Role,
                IsActive = s.IsActive,
                JoinedAt = s.JoinedAt
            });
        }

        public async Task RemoveStaffAsync(Guid storeId, Guid ownerId, Guid staffId)
        {
            var store = await _context.Store
                .FirstOrDefaultAsync(s => s.Id == storeId && s.OwnerId == ownerId);

            if (store == null)
            {
                throw new Exception("Toko tidak ditemukan atau Anda bukan pemilik toko ini.");
            }

            var staff = await _context.StoreStaffs
                .FirstOrDefaultAsync(s => s.Id == staffId && s.StoreId == storeId);

            if (staff == null)
            {
                throw new Exception("Staff tidak ditemukan.");
            }

            // Remove the User record as well (or deactivate it)
            var user = await _context.User.FindAsync(staff.UserId);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            _context.StoreStaffs.Remove(staff);
            await _context.SaveChangesAsync();
        }

        public async Task<StoreStaffResponseDto> UpdateStaffRoleAsync(Guid storeId, Guid ownerId, Guid staffId, StoreStaffUpdateDto request)
        {
            var store = await _context.Store
                .FirstOrDefaultAsync(s => s.Id == storeId && s.OwnerId == ownerId);

            if (store == null)
            {
                throw new Exception("Toko tidak ditemukan atau Anda bukan pemilik toko ini.");
            }

            var staff = await _context.StoreStaffs
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == staffId && s.StoreId == storeId);

            if (staff == null)
            {
                throw new Exception("Staff tidak ditemukan.");
            }

            staff.Role = request.Role;
            staff.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            return new StoreStaffResponseDto
            {
                Id = staff.Id,
                StoreId = staff.StoreId,
                UserId = staff.UserId,
                FullName = staff.User?.FullName ?? "Unknown",
                Email = staff.User?.Email ?? "Unknown",
                Role = staff.Role,
                IsActive = staff.IsActive,
                JoinedAt = staff.JoinedAt
            };
        }
    }
}
