using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyMenus([FromQuery] string appType)
        {
            if (string.IsNullOrEmpty(appType))
            {
                return BadRequest(new { success = false, message = "appType is required" });
            }

            // Extract claims from JWT Token
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            var membershipTierClaim = User.FindFirst("MembershipTier")?.Value;

            // Optional: fallback to Regular if no membership claim
            var membershipTier = string.IsNullOrEmpty(membershipTierClaim) ? "Regular" : membershipTierClaim;
            var role = string.IsNullOrEmpty(roleClaim) ? "Customer" : roleClaim;

            var menus = await _menuService.GetMenusAsync(appType, role, membershipTier);

            return Ok(new
            {
                success = true,
                message = "Menus retrieved successfully",
                data = menus
            });
        }
    }
}
