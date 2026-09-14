using MauNyuci.Api.Models;

namespace MauNyuci.Api.Repositories.Interfaces
{
    public interface IMenuRepository
    {
        Task<IEnumerable<AppMenu>> GetMenusAsync(string appType, string role, string membershipTier);
    }
}
