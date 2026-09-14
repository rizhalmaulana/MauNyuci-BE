using MauNyuci.Api.DTOs.Menu;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using MauNyuci.Api.Services.Interfaces;

namespace MauNyuci.Api.Services.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<AppMenuDto>> GetMenusAsync(string appType, string role, string membershipTier)
        {
            var menus = await _menuRepository.GetMenusAsync(appType, role, membershipTier);

            // Mapping to DTO and building tree hierarchy if there are sub-menus
            var menuList = menus.ToList();
            var dtos = new List<AppMenuDto>();

            // Get top-level menus
            var topLevelMenus = menuList.Where(m => m.ParentId == null).OrderBy(m => m.SortOrder);

            foreach (var menu in topLevelMenus)
            {
                dtos.Add(BuildMenuTree(menu, menuList));
            }

            return dtos;
        }

        private AppMenuDto BuildMenuTree(AppMenu menu, List<AppMenu> allMenus)
        {
            var dto = new AppMenuDto
            {
                Id = menu.Id,
                Title = menu.Title,
                Path = menu.Path,
                Icon = menu.Icon,
                SortOrder = menu.SortOrder,
                SubMenus = new List<AppMenuDto>()
            };

            var children = allMenus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.SortOrder);
            foreach (var child in children)
            {
                dto.SubMenus.Add(BuildMenuTree(child, allMenus));
            }

            return dto;
        }
    }
}
