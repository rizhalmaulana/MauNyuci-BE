using FirebaseAdmin.Messaging;
using MauNyuci.Api.Services.Interfaces;

namespace MauNyuci.Api.Services.Implementations
{
    public class FCMService : IFCMService
    {
        private readonly ILogger<FCMService> _logger;

        public FCMService(ILogger<FCMService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendPushNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null)
        {
            try
            {
                if (string.IsNullOrEmpty(fcmToken))
                    return false;

                var message = new Message()
                {
                    Token = fcmToken,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = title,
                        Body = body
                    },
                    Data = data ?? new Dictionary<string, string>()
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation("FCM Push sent successfully. Response: {Response}", response);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending FCM Push Notification to token: {Token}", fcmToken);
                return false;
            }
        }
    }
}
