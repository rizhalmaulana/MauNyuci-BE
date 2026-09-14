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
    public class StoreExpenseController : ControllerBase
    {
        private readonly IStoreExpenseService _expenseService;
        private readonly IStoreService _storeService;

        public StoreExpenseController(IStoreExpenseService expenseService, IStoreService storeService)
        {
            _expenseService = expenseService;
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
        public async Task<IActionResult> CreateExpense([FromBody] StoreExpenseRequestDto request)
        {
            try
            {
                var storeId = await GetMyStoreIdAsync();
                var result = await _expenseService.CreateExpenseAsync(storeId, request);
                return Ok(new { message = "Pengeluaran berhasil dicatat.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var storeId = await GetMyStoreIdAsync();
                var result = await _expenseService.GetExpensesAsync(storeId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] StoreExpenseRequestDto request)
        {
            try
            {
                var storeId = await GetMyStoreIdAsync();
                var result = await _expenseService.UpdateExpenseAsync(storeId, id, request);
                return Ok(new { message = "Pengeluaran berhasil diperbarui.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            try
            {
                var storeId = await GetMyStoreIdAsync();
                await _expenseService.DeleteExpenseAsync(storeId, id);
                return Ok(new { message = "Pengeluaran berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
