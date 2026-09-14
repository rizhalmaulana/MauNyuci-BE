using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MauNyuci.Api.Data;

namespace MauNyuci.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly AppDbContext _context;

        public NotificationController(INotificationRepository notificationRepo, AppDbContext context)
        {
            _notificationRepo = notificationRepo;
            _context = context;
        }

        // GET: api/notification
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized(new { success = false, message = "Token tidak valid." });

            var notifications = await _notificationRepo.GetByUserIdAsync(userId, page, pageSize);
            return Ok(new
            {
                success = true,
                message = "Berhasil mengambil data notifikasi",
                data = notifications
            });
        }

        // GET: api/notification/unread-count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized(new { success = false, message = "Token tidak valid." });

            var count = await _notificationRepo.GetUnreadCountAsync(userId);
            return Ok(new { success = true, data = new { unreadCount = count } });
        }

        // PUT: api/notification/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized(new { success = false, message = "Token tidak valid." });

            var notification = await _notificationRepo.GetByIdAsync(id);
            if (notification == null)
                return NotFound(new { success = false, message = "Notifikasi tidak ditemukan." });

            if (notification.UserId != userId)
                return Forbid();

            notification.IsRead = true;
            await _notificationRepo.UpdateAsync(notification);

            return Ok(new { success = true, message = "Notifikasi telah dibaca." });
        }

        // POST: api/notification/fcm-token
        [HttpPost("fcm-token")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] FcmTokenRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized(new { success = false, message = "Token tidak valid." });

            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest(new { success = false, message = "Token FCM tidak boleh kosong." });

            var user = await _context.User.FindAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "User tidak ditemukan." });

            user.FcmToken = request.Token;
            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "FCM Token berhasil diperbarui." });
        }
    }

    public class FcmTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
