using MauNyuci.Api.DTOs.Order;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Wajib login untuk transaksi
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequestDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.CreateOrderAsync(userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("pos-checkout")]
        public async Task<IActionResult> CreatePosOrder([FromBody] OrderPosRequestDto request)
        {
            try
            {
                var storeOwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.CreatePosOrderAsync(storeOwnerId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{orderId}/confirm-weight")]
        public async Task<IActionResult> UpdateWeightOrder(Guid orderId, [FromBody] OrderConfirmRequestDto request)
        {
            try
            {
                var storeOwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.UpdateWeightAsync(orderId, request, storeOwnerId);
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

        [HttpPut("{orderId}/confirm-pickup")]
        public async Task<IActionResult> ConfirmPickup(Guid orderId, [FromBody] AssignDriverDto request)
        {
            try
            {
                var storeOwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.ConfirmPickupAsync(orderId, storeOwnerId, request.DriverId);
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

        [HttpPut("{orderId}/ready-for-delivery")]
        public async Task<IActionResult> ReadyForDelivery(Guid orderId, [FromBody] AssignDriverDto request)
        {
            try
            {
                var storeOwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.ReadyForDeliveryAsync(orderId, storeOwnerId, request.DriverId);
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

        [HttpPut("{orderId}/change-payment-method")]
        public async Task<IActionResult> ChangePaymentMethod(Guid orderId, [FromBody] ChangePaymentMethodDto request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.ChangePaymentMethodAsync(orderId, customerId, request.NewMethod);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{orderId}/accept")]
        public async Task<IActionResult> AcceptOrder(Guid orderId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.AcceptOrderAsync(orderId, userId);
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

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.CancelOrderAsync(orderId, userId);
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

        [HttpPut("{orderId}/finish-washing")]
        public async Task<IActionResult> FinishWashing(Guid orderId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.FinishWashingAsync(orderId, userId);
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

        [HttpPut("{orderId}/complete")]
        public async Task<IActionResult> CompleteOrder(Guid orderId, [FromBody] OrderCompleteRequestDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.CompleteOrderAsync(orderId, userId, request);
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

        [HttpGet("customer")]
        public async Task<IActionResult> GetMyOrders([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.GetCustomerOrdersAsync(userId, status, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetStoreOrders(Guid storeId)
        {
            try
            {
                // Ambil ID User dari Token JWT (Kasir/Owner yang sedang login)
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.GetStoreOrdersAsync(storeId, userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Kembalikan 403 Forbidden jika dia mencoba intip toko orang lain
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("store/{storeId}/history")]
        public async Task<IActionResult> GetStoreOrderHistory(
            Guid storeId, 
            [FromQuery] string? search, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.GetStoreOrderHistoryAsync(storeId, userId, search, startDate, endDate, page, pageSize);
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

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(Guid orderId)
        {
            try
            {
                // Ambil ID dari token JWT yang sedang login
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var result = await _orderService.GetOrderByIdAsync(orderId, userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Lempar status 403 jika ada yang mencoba mengintip
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{orderId}/customer-cancel")]
        [Authorize]
        public async Task<IActionResult> CustomerCancelOrder(Guid orderId, [FromBody] OrderCancelRequestDto request)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.CustomerCancelOrderAsync(orderId, customerId, request);
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

        [HttpPost("{orderId}/upload-receipt")]
        public async Task<IActionResult> UploadPaymentReceipt(Guid orderId, IFormFile file)
        {
            try
            {
                var customerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.UploadPaymentReceiptAsync(orderId, customerId, file);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPost("store/{orderId}/upload-receipt")]
        public async Task<IActionResult> StoreUploadPaymentReceipt(Guid orderId, IFormFile file)
        {
            try
            {
                var storeOwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.StoreUploadPaymentReceiptAsync(orderId, storeOwnerId, file);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{orderId}/verify-payment")]
        public async Task<IActionResult> VerifyPayment(Guid orderId, [FromBody] VerifyPaymentRequestDto request)
        {
            try
            {
                var storeOwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.VerifyPaymentAsync(orderId, storeOwnerId, request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{orderId}/driver-confirm-cash")]
        public async Task<IActionResult> ConfirmDriverCashPayment(Guid orderId)
        {
            try
            {
                var driverId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                // Nantinya bisa ditambahkan validasi apakah user ini benar-benar role Driver
                var result = await _orderService.ConfirmDriverCashPaymentAsync(orderId, driverId);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}