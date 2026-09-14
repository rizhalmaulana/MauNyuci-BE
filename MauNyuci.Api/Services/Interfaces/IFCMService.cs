namespace MauNyuci.Api.Services.Interfaces
{
    public interface IFCMService
    {
        Task<bool> SendPushNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null);
    }
}
