using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs.Review;
using MauNyuci.Api.Models;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReviewResponseDto> AddReviewAsync(Guid customerId, ReviewCreateRequestDto request)
        {
            // Cari pesanan
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            if (order == null)
                throw new Exception("Pesanan tidak ditemukan.");

            // Validasi Kepemilikan: Hanya yang pesan yang boleh mereview
            if (order.CustomerId != customerId)
                throw new UnauthorizedAccessException("Anda tidak berhak memberikan ulasan untuk pesanan ini.");

            // Validasi Status: Cucian harus sudah selesai
            if (order.Status != OrderStatus.Completed)
                throw new Exception("Anda hanya bisa memberikan ulasan setelah pesanan selesai (Completed).");

            // Validasi Duplikasi: Cek apakah order ini sudah pernah direview
            var existingReview = await _context.Reviews.AnyAsync(r => r.OrderId == request.OrderId);
            if (existingReview)
                throw new Exception("Anda sudah memberikan ulasan untuk pesanan ini.");

            // Simpan Review
            var review = new Review
            {
                OrderId = order.Id,
                StoreId = order.StoreId,
                CustomerId = customerId,
                Rating = request.Rating,
                Comment = request.Comment
            };

            _context.Reviews.Add(review);

            var store = await _context.Store.FirstOrDefaultAsync(s => s.Id == order.StoreId);
            if (store != null)
            {
                // Rumus Incremental Average:
                // NewAverage = ((CurrentAvg * CurrentTotal) + NewRating) / (CurrentTotal + 1)

                double totalRatingPoint = (store.AverageRating * store.TotalReviews) + request.Rating;
                store.TotalReviews += 1;
                store.AverageRating = Math.Round(totalRatingPoint / store.TotalReviews, 1); // Bulatkan 1 angka di belakang koma

                _context.Store.Update(store);
            }

            await _context.SaveChangesAsync();

            return new ReviewResponseDto
            {
                Id = review.Id,
                OrderId = review.OrderId,
                CustomerName = order.Customer?.FullName ?? "Customer",
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetStoreReviewsAsync(Guid storeId)
        {
            // Ambil ulasan dari yang terbaru
            var reviews = await _context.Reviews
                .Include(r => r.Customer)
                .Where(r => r.StoreId == storeId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                OrderId = r.OrderId,
                CustomerName = r.Customer?.FullName ?? "Customer",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }
    }
}