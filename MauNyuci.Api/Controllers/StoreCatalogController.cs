using MauNyuci.Api.DTOs.StoreCatalog;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreCatalogController : ControllerBase
    {
        private readonly IStoreCatalogService _catalogService;

        public StoreCatalogController(IStoreCatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        // Endpoint untuk mendapatkan daftar pilihan icon yang tersedia (Statis)
        [HttpGet("available-icons")]
        public IActionResult GetAvailableIcons()
        {
            var icons = new[]
            {
                "img_batik.png",
                "img_bedcover.png",
                "img_boneka.png",
                "img_celana.png",
                "img_hoodie.png",
                "img_jas.png",
                "img_jeans.png",
                "img_kaos_satuan.png",
                "img_kemeja_satuan.png",
                "img_laundry_item.png",
                "img_selimut.png",
                "img_sepatu.png"
            };

            return Ok(new 
            { 
                message = "Daftar icon berhasil dimuat.", 
                data = icons 
            });
        }

        // Endpoint untuk PEMILIK TOKO menambah layanan (Wajib Login)
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddCatalogItem([FromBody] StoreCatalogCreateDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _catalogService.AddCatalogItemAsync(userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint untuk CUSTOMER melihat daftar layanan toko tertentu (Bisa dipanggil tanpa login)
        [HttpGet("{storeId}")]
        public async Task<IActionResult> GetStoreCatalogs(Guid storeId)
        {
            var result = await _catalogService.GetStoreCatalogsAsync(storeId);
            return Ok(result);
        }

        // Endpoint untuk PEMILIK TOKO melihat daftar layanan toko miliknya sendiri
        [HttpGet("my-catalog")]
        [Authorize]
        public async Task<IActionResult> GetMyStoreCatalog()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _catalogService.GetMyStoreCatalogAsync(userId);

                if (result == null || !result.Any())
                {
                    return Ok(new 
                    { 
                        message = "Data katalog belum diinputkan / kosong.", 
                        data = result ?? Enumerable.Empty<StoreCatalogResponseDto>()
                    });
                }

                return Ok(new 
                { 
                    message = "Data katalog berhasil dimuat.", 
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint untuk PEMILIK TOKO mengubah layanan
        [HttpPut("update/{catalogId}")]
        [Authorize]
        public async Task<IActionResult> UpdateCatalogItem(Guid catalogId, [FromBody] StoreCatalogUpdateDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _catalogService.UpdateCatalogItemAsync(userId, catalogId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint untuk PEMILIK TOKO menghapus layanan
        [HttpDelete("delete/{catalogId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCatalogItem(Guid catalogId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _catalogService.DeleteCatalogItemAsync(userId, catalogId);
                return Ok(new { message = "Layanan berhasil dihapus" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}