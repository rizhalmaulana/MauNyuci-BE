using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs.Membership;
using MauNyuci.Api.Models;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Services.Implementations
{
    public class StoreAnalyticsService : IStoreAnalyticsService
    {
        private readonly AppDbContext _context;

        public StoreAnalyticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardAnalyticsResponseDto> GetDashboardAnalyticsAsync(Guid storeId, DateTime? startDate, DateTime? endDate)
        {
            var orderQuery = _context.Orders
                .Where(o => o.StoreId == storeId && o.Status == OrderStatus.Completed);

            var expenseQuery = _context.StoreExpenses
                .Where(e => e.StoreId == storeId);

            if (startDate.HasValue)
            {
                var startUtc = startDate.Value.ToUniversalTime();
                orderQuery = orderQuery.Where(o => o.CreatedAt >= startUtc);
                expenseQuery = expenseQuery.Where(e => e.ExpenseDate >= startUtc);
            }

            if (endDate.HasValue)
            {
                var endUtc = endDate.Value.ToUniversalTime();
                orderQuery = orderQuery.Where(o => o.CreatedAt <= endUtc);
                expenseQuery = expenseQuery.Where(e => e.ExpenseDate <= endUtc);
            }

            var orders = await orderQuery.Include(o => o.OrderItems).ToListAsync();
            var expenses = await expenseQuery.ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalExpense = expenses.Sum(e => e.Amount);
            var orderCount = orders.Count;

            var topSelling = orders.SelectMany(o => o.OrderItems)
                .GroupBy(i => i.ItemName)
                .Select(g => new TopSellingServiceDto
                {
                    ServiceName = g.Key,
                    TotalSold = g.Count(),
                    TotalRevenue = g.Sum(i => i.SubTotal)
                })
                .OrderByDescending(t => t.TotalRevenue)
                .Take(5)
                .ToList();

            var peakHours = orders
                .GroupBy(o => o.CreatedAt.ToLocalTime().Hour)
                .Select(g => new PeakHourDto
                {
                    Hour = g.Key,
                    OrderCount = g.Count()
                })
                .OrderBy(p => p.Hour)
                .ToList();

            // Daily Trend (Simple implementation grouping by Date)
            var revenueTrend = orders
                .GroupBy(o => o.CreatedAt.ToLocalTime().Date)
                .Select(g => new TrendDataDto
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Value = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(t => t.Label)
                .ToList();

            var expenseTrend = expenses
                .GroupBy(e => e.ExpenseDate.ToLocalTime().Date)
                .Select(g => new TrendDataDto
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Value = g.Sum(e => e.Amount)
                })
                .OrderBy(t => t.Label)
                .ToList();

            return new DashboardAnalyticsResponseDto
            {
                TotalRevenue = totalRevenue,
                TotalExpense = totalExpense,
                NetProfit = totalRevenue - totalExpense,
                OrderCount = orderCount,
                AverageOrderValue = orderCount > 0 ? totalRevenue / orderCount : 0,
                TopSellingServices = topSelling,
                PeakHours = peakHours,
                RevenueTrend = revenueTrend,
                ExpenseTrend = expenseTrend
            };
        }
    }
}
