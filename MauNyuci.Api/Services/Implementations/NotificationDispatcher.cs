using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using MauNyuci.Api.Services.Interfaces;
using MauNyuci.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using MauNyuci.Api.Data;

namespace MauNyuci.Api.Services.Implementations
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly AppDbContext _dbContext;
        private readonly IFCMService _fcmService;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly ILogger<NotificationDispatcher> _logger;

        public NotificationDispatcher(
            INotificationRepository notificationRepo,
            AppDbContext dbContext,
            IFCMService fcmService,
            IHubContext<OrderHub> hubContext,
            ILogger<NotificationDispatcher> logger)
        {
            _notificationRepo = notificationRepo;
            _dbContext = dbContext;
            _fcmService = fcmService;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task DispatchNotificationAsync(Guid userId, string title, string message, Dictionary<string, string>? data = null)
        {
            try 
            {
                // 1. Simpan ke Database
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _notificationRepo.CreateAsync(notification);

                // 2. Kirim via SignalR WebSocket (Real-time untuk user yang sedang membuka aplikasi)
                await _hubContext.Clients.Group($"User-{userId}").SendAsync("ReceiveNotification", notification);

                // 3. Kirim via FCM (Push Notification ke OS Android)
                var user = await _dbContext.User.FindAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.FcmToken))
                {
                    await _fcmService.SendPushNotificationAsync(user.FcmToken, title, message, data);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch notification for UserId {UserId}", userId);
            }
        }
    }
}
