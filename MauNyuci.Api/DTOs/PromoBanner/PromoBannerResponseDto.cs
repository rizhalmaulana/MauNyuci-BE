namespace MauNyuci.Api.DTOs.PromoBanner
{
    public class PromoBannerResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string CtaText { get; set; } = string.Empty;
        public string CtaType { get; set; } = string.Empty;
        public string CtaValue { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public string[] TargetRoles { get; set; } = Array.Empty<string>();
        public string[] TargetTiers { get; set; } = Array.Empty<string>();
        public string AppType { get; set; } = string.Empty;
    }
}
