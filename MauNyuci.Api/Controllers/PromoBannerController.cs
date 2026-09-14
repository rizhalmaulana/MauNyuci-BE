using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs.PromoBanner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoBannersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PromoBannersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPromoBanners([FromQuery] string? appType)
        {
            try
            {
                var query = _context.PromoBanners.Where(b => b.IsActive);

                if (!string.IsNullOrEmpty(appType))
                {
                    query = query.Where(b => b.AppType == appType);
                }

                var banners = await query.OrderBy(b => b.SortOrder).ToListAsync();

                var dtos = banners.Select(b => new PromoBannerResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Icon = b.Icon,
                    ImageUrl = b.ImageUrl,
                    CtaText = b.CtaText,
                    CtaType = b.CtaType,
                    CtaValue = b.CtaValue,
                    SortOrder = b.SortOrder,
                    IsActive = b.IsActive,
                    TargetRoles = b.TargetRoles,
                    TargetTiers = b.TargetTiers,
                    AppType = b.AppType
                });

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = dtos
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
