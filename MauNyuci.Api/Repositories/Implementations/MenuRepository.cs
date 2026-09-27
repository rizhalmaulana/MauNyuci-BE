using Microsoft.EntityFrameworkCore;
using MauNyuci.Api.Data;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;

namespace MauNyuci.Api.Repositories.Implementations
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppMenu>> GetMenusAsync(string appType, string role, string membershipTier)
        {
            return await _context.AppMenus
                .Where(m => m.IsActive 
                         && m.AppType == appType
                         && (string.IsNullOrEmpty(m.RequiredRole) 
                             || m.RequiredRole == role 
                             || (m.RequiredRole == "Store" && (role == "Owner" || role == "StoreStaff")))
                         && (m.RequiredMembershipTierId == null || m.RequiredMembershipTier!.Name == membershipTier))
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }
    }
}
