using MauNyuci.Api.DTOs.Review;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> AddReviewAsync(Guid customerId, ReviewCreateRequestDto request);
        Task<IEnumerable<ReviewResponseDto>> GetStoreReviewsAsync(Guid storeId);
    }
}