using MauNyuci.Api.Attributes;
using MauNyuci.Api.DTOs.Membership;
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
    public class StorePromoController : ControllerBase
    {
        private readonly IStorePromoService _promoService;
        private readonly IStoreService _storeService;

        public StorePromoController(IStorePromoService promoService, IStoreService storeService)
        {
            _promoService = promoService;
            _storeService = storeService;
        }

        private async Task<Guid> GetMyStoreIdAsync()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var store = await _storeService.GetMyStoreAsync(userId);
            if (store == null) throw new Exception("Toko tidak ditemukan.");
            return store.Id;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromo([FromBody] StorePromoRequestDto request)
        {
            try
            {
                var storeId = await GetMyStoreIdAsync();
                var result = await _promoService.CreatePromoAsync(storeId, request);
                return Ok(new { message = "Promo berhasil dibuat.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPromos()
        {
            try
            {
                var storeId = await GetMyStoreIdAsync();
                var result = await _promoService.GetPromosAsync(storeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromo(Guid id, [FromBody] StorePromoRequestDto request)
        {
            try
            {
                var storeId = await GetMyStoreIdAsync();
                var result = await _promoService.UpdatePromoAsync(storeId, id, request);
                return Ok(new { message = "Promo berhasil diperbarui.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromo(Guid id)
        {
            try
            {
                var storeId = await GetMyStoreIdAsync();
                await _promoService.DeletePromoAsync(storeId, id);
                return Ok(new { message = "Promo berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
