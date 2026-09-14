using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs.Membership;
using MauNyuci.Api.Models;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Services.Implementations
{
    public class StorePromoService : IStorePromoService
    {
        private readonly AppDbContext _context;

        public StorePromoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StorePromoResponseDto> CreatePromoAsync(Guid storeId, StorePromoRequestDto request)
        {
            var existing = await _context.StorePromos.FirstOrDefaultAsync(p => p.StoreId == storeId && p.PromoCode == request.PromoCode);
            if (existing != null) throw new Exception("Kode promo ini sudah digunakan di toko Anda.");

            var promo = new StorePromo
            {
                StoreId = storeId,
                PromoCode = request.PromoCode.ToUpper(),
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MaxDiscountAmount = request.DiscountType == "Percentage" ? request.MaxDiscountAmount : null,
                MinOrderAmount = request.MinOrderAmount,
                ExpiryDate = request.ExpiryDate.ToUniversalTime(),
                IsActive = request.IsActive,
                DiscountTarget = request.DiscountTarget
            };

            _context.StorePromos.Add(promo);
            await _context.SaveChangesAsync();

            return MapToDto(promo);
        }

        public async Task<List<StorePromoResponseDto>> GetPromosAsync(Guid storeId)
        {
            var promos = await _context.StorePromos
                .Where(p => p.StoreId == storeId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return promos.Select(MapToDto).ToList();
        }

        public async Task<StorePromoResponseDto> UpdatePromoAsync(Guid storeId, Guid promoId, StorePromoRequestDto request)
        {
            var promo = await _context.StorePromos.FirstOrDefaultAsync(p => p.StoreId == storeId && p.Id == promoId);
            if (promo == null) throw new Exception("Promo tidak ditemukan.");

            promo.PromoCode = request.PromoCode.ToUpper();
            promo.DiscountType = request.DiscountType;
            promo.DiscountValue = request.DiscountValue;
            promo.MaxDiscountAmount = request.DiscountType == "Percentage" ? request.MaxDiscountAmount : null;
            promo.MinOrderAmount = request.MinOrderAmount;
            promo.ExpiryDate = request.ExpiryDate.ToUniversalTime();
            promo.IsActive = request.IsActive;
            promo.DiscountTarget = request.DiscountTarget;
            promo.UpdatedAt = DateTime.UtcNow;

            _context.StorePromos.Update(promo);
            await _context.SaveChangesAsync();

            return MapToDto(promo);
        }

        public async Task<bool> DeletePromoAsync(Guid storeId, Guid promoId)
        {
            var promo = await _context.StorePromos.FirstOrDefaultAsync(p => p.StoreId == storeId && p.Id == promoId);
            if (promo == null) throw new Exception("Promo tidak ditemukan.");

            _context.StorePromos.Remove(promo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<StorePromoResponseDto?> ValidatePromoAsync(Guid storeId, string promoCode, decimal orderAmount)
        {
            var promo = await _context.StorePromos.FirstOrDefaultAsync(p => p.StoreId == storeId && p.PromoCode == promoCode.ToUpper() && p.IsActive);
            
            if (promo == null) throw new Exception("Promo tidak valid atau tidak ditemukan.");
            if (promo.ExpiryDate < DateTime.UtcNow) throw new Exception("Promo sudah kedaluwarsa.");
            if (orderAmount < promo.MinOrderAmount) throw new Exception($"Minimal transaksi untuk promo ini adalah {promo.MinOrderAmount}.");

            return MapToDto(promo);
        }

        private StorePromoResponseDto MapToDto(StorePromo promo)
        {
            return new StorePromoResponseDto
            {
                Id = promo.Id,
                PromoCode = promo.PromoCode,
                DiscountType = promo.DiscountType,
                DiscountValue = promo.DiscountValue,
                MaxDiscountAmount = promo.MaxDiscountAmount,
                MinOrderAmount = promo.MinOrderAmount,
                ExpiryDate = promo.ExpiryDate,
                IsActive = promo.IsActive,
                DiscountTarget = promo.DiscountTarget,
                CreatedAt = promo.CreatedAt
            };
        }
    }
}
