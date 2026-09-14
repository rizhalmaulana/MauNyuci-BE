using MauNyuci.Api.Models;

namespace MauNyuci.Api.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification> CreateAsync(Notification notification);
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<Notification?> GetByIdAsync(Guid id);
        Task<Notification> UpdateAsync(Notification notification);
    }
}
