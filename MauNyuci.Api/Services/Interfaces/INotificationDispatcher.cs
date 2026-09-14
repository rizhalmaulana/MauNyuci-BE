namespace MauNyuci.Api.Services.Interfaces
{
    public interface INotificationDispatcher
    {
        Task DispatchNotificationAsync(Guid userId, string title, string message, Dictionary<string, string>? data = null);
    }
}
