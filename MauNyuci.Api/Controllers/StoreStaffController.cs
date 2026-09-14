using MauNyuci.Api.DTOs.StoreStaff;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MauNyuci.Api.Controllers
{
    [Route("api/stores/{storeId}/staffs")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class StoreStaffController : ControllerBase
    {
        private readonly IStoreStaffService _storeStaffService;

        public StoreStaffController(IStoreStaffService storeStaffService)
        {
            _storeStaffService = storeStaffService;
        }

        [HttpPost]
        public async Task<IActionResult> AddStaff(Guid storeId, [FromBody] StoreStaffCreateDto request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid ownerId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            try
            {
                var staff = await _storeStaffService.AddStaffAsync(storeId, ownerId, request);
                return CreatedAtAction(nameof(GetStaffs), new { storeId }, staff);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffs(Guid storeId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid ownerId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            try
            {
                var staffs = await _storeStaffService.GetStoreStaffsAsync(storeId, ownerId);
                return Ok(new { data = staffs });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{staffId}")]
        public async Task<IActionResult> UpdateStaff(Guid storeId, Guid staffId, [FromBody] StoreStaffUpdateDto request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid ownerId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            try
            {
                var staff = await _storeStaffService.UpdateStaffRoleAsync(storeId, ownerId, staffId, request);
                return Ok(new { data = staff });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{staffId}")]
        public async Task<IActionResult> RemoveStaff(Guid storeId, Guid staffId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid ownerId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            try
            {
                await _storeStaffService.RemoveStaffAsync(storeId, ownerId, staffId);
                return Ok(new { message = "Staff removed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
