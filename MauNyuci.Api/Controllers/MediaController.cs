using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            try
            {
                // Upload gambar ke folder 'profiles' di Cloudflare R2
                var imageUrl = await _mediaService.UploadImageAsync(file, "profiles");

                // Catatan: Setelah dapat imageUrl ini, Flutter bisa memanggil API UpdateProfile 
                // untuk menyimpan URL ini ke database User.

                return Ok(new { message = "Upload berhasil", url = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("upload-catalog-image")]
        public async Task<IActionResult> UploadCatalogImage(IFormFile file)
        {
            try
            {
                // Upload gambar ke folder 'catalogs' di Cloudflare R2
                var imageUrl = await _mediaService.UploadImageAsync(file, "catalogs");
                
                return Ok(new { message = "Upload berhasil", url = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("upload-qris-image")]
        public async Task<IActionResult> UploadQrisImage(IFormFile file)
        {
            try
            {
                // Upload gambar ke folder 'qris' di Cloudflare R2
                var imageUrl = await _mediaService.UploadImageAsync(file, "qris");
                
                return Ok(new { message = "Upload berhasil", url = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}