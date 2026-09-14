namespace MauNyuci.Api.Services.Interfaces
{
    public interface IReminderJobService
    {
        Task SendPaymentReminderAsync(Guid orderId);
        Task SendReviewReminderAsync(Guid orderId);
        Task SendStoreSLAReminderAsync(Guid orderId);
    }
}
