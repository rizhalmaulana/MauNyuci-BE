using MauNyuci.Api.DTOs.Menu;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IMenuService
    {
        Task<IEnumerable<AppMenuDto>> GetMenusAsync(string appType, string role, string membershipTier);
    }
}
