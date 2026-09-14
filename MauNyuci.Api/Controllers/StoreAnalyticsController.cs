using MauNyuci.Api.Attributes;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequireMembership("Premium")]
    public class StoreAnalyticsController : ControllerBase
    {
        private readonly IStoreAnalyticsService _analyticsService;
        private readonly IStoreService _storeService;

        public StoreAnalyticsController(IStoreAnalyticsService analyticsService, IStoreService storeService)
        {
            _analyticsService = analyticsService;
            _storeService = storeService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var store = await _storeService.GetMyStoreAsync(userId);
                if (store == null) return NotFound(new { message = "Toko tidak ditemukan." });

                var result = await _analyticsService.GetDashboardAnalyticsAsync(store.Id, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
