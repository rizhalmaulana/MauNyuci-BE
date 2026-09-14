namespace MauNyuci.Api.DTOs.Menu
{
    public class AppMenuDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
        public ICollection<AppMenuDto>? SubMenus { get; set; }
    }
}
