using System.Security.Claims;
using MauNyuci.Api.DTOs.Store;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreBankAccountController : ControllerBase
    {
        private readonly IStoreBankAccountService _bankAccountService;

        public StoreBankAccountController(IStoreBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        // Endpoint statis untuk daftar metode pembayaran master
        [HttpGet("master-methods")]
        public IActionResult GetMasterMethods()
        {
            var methods = new[]
            {
                "BCA", "Mandiri", "BNI", "BRI", "CIMB Niaga", 
                "GoPay", "OVO", "Dana", "ShopeePay", "LinkAja", "QRIS", "Tunai"
            };

            return Ok(new
            {
                message = "Daftar metode pembayaran berhasil dimuat.",
                data = methods
            });
        }

        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetStoreAccounts(Guid storeId)
        {
            try
            {
                var accounts = await _bankAccountService.GetStoreBankAccountsAsync(storeId);
                return Ok(new { message = "Data rekening toko berhasil dimuat.", data = accounts });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-accounts")]
        [Authorize]
        public async Task<IActionResult> GetMyAccounts()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var accounts = await _bankAccountService.GetMyBankAccountsAsync(userId);
                return Ok(new { message = "Data rekening berhasil dimuat.", data = accounts });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddAccount([FromBody] StoreBankAccountCreateDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var created = await _bankAccountService.AddBankAccountAsync(userId, request);
                return Ok(new { message = "Rekening berhasil ditambahkan.", data = created });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] StoreBankAccountUpdateDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var updated = await _bankAccountService.UpdateBankAccountAsync(userId, id, request);
                return Ok(new { message = "Rekening berhasil diperbarui.", data = updated });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _bankAccountService.DeleteBankAccountAsync(userId, id);
                return Ok(new { message = "Rekening berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
