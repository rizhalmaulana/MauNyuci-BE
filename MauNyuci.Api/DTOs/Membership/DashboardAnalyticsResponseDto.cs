namespace MauNyuci.Api.DTOs.Membership
{
    public class DashboardAnalyticsResponseDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetProfit { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }

        public List<TopSellingServiceDto> TopSellingServices { get; set; } = new();
        public List<TrendDataDto> RevenueTrend { get; set; } = new();
        public List<TrendDataDto> ExpenseTrend { get; set; } = new();
        public List<PeakHourDto> PeakHours { get; set; } = new();
    }

    public class TopSellingServiceDto
    {
        public string ServiceName { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TrendDataDto
    {
        public string Label { get; set; } = string.Empty; // e.g. "2026-09-01" or "08:00"
        public decimal Value { get; set; }
    }

    public class PeakHourDto
    {
        public int Hour { get; set; } // 0-23
        public int OrderCount { get; set; }
    }
}
