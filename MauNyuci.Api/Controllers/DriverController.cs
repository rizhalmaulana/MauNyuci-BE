using MauNyuci.Api.DTOs.Driver;
using MauNyuci.Api.Services.Implementations;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Wajib login
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly IOrderService _orderService;

        public DriverController(IDriverService driverService, IOrderService orderService)
        {
            _driverService = driverService;
            _orderService = orderService;
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var tasks = await _driverService.GetMyTasksAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{orderId}/pickup")]
        public async Task<IActionResult> ConfirmPickup(Guid orderId, IFormFile evidenceFile)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Panggil fungsi service dengan menyertakan evidenceFile
                var result = await _driverService.ConfirmPickupAsync(orderId, userId, evidenceFile);

                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{orderId}/deliver-cash")]
        public async Task<IActionResult> ConfirmDeliveryAndCashPayment(Guid orderId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _driverService.ConfirmDeliveryAndCashPaymentAsync(orderId, userId);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{orderId}/deliver-photo")]
        public async Task<IActionResult> ConfirmDeliveryWithPhoto(Guid orderId, [FromForm] DeliveryPhotoRequestDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Akses file dan note dari object request
                var result = await _driverService.ConfirmDeliveryWithPhotoAsync(
                    orderId,
                    userId,
                    request.EvidenceFile,
                    request.Note ?? "");

                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _driverService.UpdateLocationAsync(userId, request.Latitude, request.Longitude);
                return Ok(new { message = "Lokasi berhasil diupdate." });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpGet("unsettled-cash")]
        public async Task<IActionResult> GetUnsettledCash()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                // Panggil fungsi dari OrderService
                var amount = await _orderService.GetUnsettledCashForDriverAsync(userId);
                return Ok(new { unsettledAmount = amount });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // PERHATIAN: Endpoint ini diakses oleh Kasir/Owner Toko untuk menerima uang dari Driver
        [HttpPost("{driverId}/settle-cash")]
        public async Task<IActionResult> SettleCashToStore(Guid driverId)
        {
            try
            {
                var storeOwnerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _driverService.SettleCashToStoreAsync(driverId, storeOwnerId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }

    // DTO Kecil untuk tangkap Latitude & Longitude dari Body
    public class UpdateLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DeliveryPhotoRequestDto
    {
        public IFormFile EvidenceFile { get; set; } = null!;
        public string? Note { get; set; }
    }
}