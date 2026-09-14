using MauNyuci.Api.DTOs.Inventory;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MauNyuci.Api.Controllers
{
    [Route("api/stores/{storeId}/inventory")]
    [ApiController]
    [Authorize(Roles = "Owner,StoreStaff")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        private Guid GetUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                throw new Exception("Invalid token");
            }
            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(Guid storeId, [FromBody] InventoryItemCreateDto request)
        {
            try
            {
                var item = await _inventoryService.CreateInventoryItemAsync(storeId, GetUserId(), request);
                return CreatedAtAction(nameof(GetItems), new { storeId }, item);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItems(Guid storeId)
        {
            try
            {
                var items = await _inventoryService.GetInventoryItemsAsync(storeId, GetUserId());
                return Ok(new { data = items });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{itemId}")]
        public async Task<IActionResult> UpdateItem(Guid storeId, Guid itemId, [FromBody] InventoryItemUpdateDto request)
        {
            try
            {
                var item = await _inventoryService.UpdateInventoryItemAsync(storeId, GetUserId(), itemId, request);
                return Ok(new { data = item });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> RemoveItem(Guid storeId, Guid itemId)
        {
            try
            {
                await _inventoryService.RemoveInventoryItemAsync(storeId, GetUserId(), itemId);
                return Ok(new { message = "Barang berhasil dihapus dari inventori" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{itemId}/transactions")]
        public async Task<IActionResult> AddTransaction(Guid storeId, Guid itemId, [FromBody] InventoryTransactionCreateDto request)
        {
            try
            {
                var transaction = await _inventoryService.AddTransactionAsync(storeId, GetUserId(), itemId, request);
                return Ok(new { data = transaction });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{itemId}/transactions")]
        public async Task<IActionResult> GetItemTransactions(Guid storeId, Guid itemId)
        {
            try
            {
                var transactions = await _inventoryService.GetItemTransactionsAsync(storeId, GetUserId(), itemId);
                return Ok(new { data = transactions });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
