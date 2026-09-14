using MauNyuci.Api.DTOs.Review;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize] // Harus login untuk memberi ulasan
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateRequestDto request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _reviewService.AddReviewAsync(customerId, request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("store/{storeId}")]
        // Tidak perlu [Authorize], siapa saja bisa melihat ulasan toko
        public async Task<IActionResult> GetStoreReviews(Guid storeId)
        {
            try
            {
                var result = await _reviewService.GetStoreReviewsAsync(storeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}