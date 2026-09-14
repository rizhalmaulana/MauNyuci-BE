using MauNyuci.Api.DTOs.Membership;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IStoreAnalyticsService
    {
        Task<DashboardAnalyticsResponseDto> GetDashboardAnalyticsAsync(Guid storeId, DateTime? startDate, DateTime? endDate);
    }
}
