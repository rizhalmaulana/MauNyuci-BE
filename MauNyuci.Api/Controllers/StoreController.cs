using System.Security.Claims;
using MauNyuci.Api.DTOs.Store;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly IOrderService _orderService;

        public StoreController(IStoreService storeService, IOrderService orderService)
        {
            _storeService = storeService;
            _orderService = orderService;
        }

        [HttpPost("register-store")]
        [Authorize] // Harus login untuk buat toko
        public async Task<IActionResult> CreateStore([FromForm] StoreCreateRequestDto request)
        {
            // Mengambil ID User dari Token JWT yang sedang login
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized(new { message = "Token tidak valid atau User tidak ditemukan." });

            var userId = Guid.Parse(userIdClaim);

            try
            {
                var result = await _storeService.CreateStoreAsync(userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllStores()
        {
            var stores = await _storeService.GetAllStoresAsync();
            return Ok(stores);
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyStores(
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] double radius = 5.0) // Default pencarian radius 5 KM
        {
            try
            {
                var stores = await _storeService.GetNearbyStoresAsync(lat, lng, radius);
                return Ok(stores);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-store")]
        [Authorize] // Wajib login
        public async Task<IActionResult> GetMyStore()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);
            var store = await _storeService.GetMyStoreAsync(userId);

            if (store == null)
            {
                // Beri tahu Flutter bahwa user ini belum buka toko
                return NotFound(new 
                { 
                    isStoreRegistered = false,
                    message = "Anda belum memiliki toko. Silakan daftar terlebih dahulu." 
                });
            }

            return Ok(store);
        }

        [HttpPut("my-store/update")]
        [Authorize] // Wajib login
        public async Task<IActionResult> UpdateStore([FromForm] StoreUpdateRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            try
            {
                var updatedStore = await _storeService.UpdateStoreAsync(userId, request);
                return Ok(new { message = "Data toko berhasil diperbarui.", data = updatedStore });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{storeId}/transactions")]
        [Authorize]
        public async Task<IActionResult> GetStoreTransactionsSummary(Guid storeId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.GetStoreTransactionsSummaryAsync(storeId, userId, startDate, endDate);
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
    }
}