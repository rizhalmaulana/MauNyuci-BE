using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs.Membership;
using MauNyuci.Api.Models;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Services.Implementations
{
    public class StoreExpenseService : IStoreExpenseService
    {
        private readonly AppDbContext _context;

        public StoreExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StoreExpenseResponseDto> CreateExpenseAsync(Guid storeId, StoreExpenseRequestDto request)
        {
            var expense = new StoreExpense
            {
                StoreId = storeId,
                Category = request.Category,
                Amount = request.Amount,
                Description = request.Description,
                ExpenseDate = request.ExpenseDate.ToUniversalTime()
            };

            _context.StoreExpenses.Add(expense);
            await _context.SaveChangesAsync();

            return MapToDto(expense);
        }

        public async Task<List<StoreExpenseResponseDto>> GetExpensesAsync(Guid storeId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.StoreExpenses.Where(e => e.StoreId == storeId).AsQueryable();

            if (startDate.HasValue)
            {
                var startUtc = startDate.Value.ToUniversalTime();
                query = query.Where(e => e.ExpenseDate >= startUtc);
            }
            if (endDate.HasValue)
            {
                var endUtc = endDate.Value.ToUniversalTime();
                query = query.Where(e => e.ExpenseDate <= endUtc);
            }

            var expenses = await query.OrderByDescending(e => e.ExpenseDate).ToListAsync();
            return expenses.Select(MapToDto).ToList();
        }

        public async Task<StoreExpenseResponseDto> UpdateExpenseAsync(Guid storeId, Guid expenseId, StoreExpenseRequestDto request)
        {
            var expense = await _context.StoreExpenses.FirstOrDefaultAsync(e => e.StoreId == storeId && e.Id == expenseId);
            if (expense == null) throw new Exception("Pengeluaran tidak ditemukan.");

            expense.Category = request.Category;
            expense.Amount = request.Amount;
            expense.Description = request.Description;
            expense.ExpenseDate = request.ExpenseDate.ToUniversalTime();
            expense.UpdatedAt = DateTime.UtcNow;

            _context.StoreExpenses.Update(expense);
            await _context.SaveChangesAsync();

            return MapToDto(expense);
        }

        public async Task<bool> DeleteExpenseAsync(Guid storeId, Guid expenseId)
        {
            var expense = await _context.StoreExpenses.FirstOrDefaultAsync(e => e.StoreId == storeId && e.Id == expenseId);
            if (expense == null) throw new Exception("Pengeluaran tidak ditemukan.");

            _context.StoreExpenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        private StoreExpenseResponseDto MapToDto(StoreExpense expense)
        {
            return new StoreExpenseResponseDto
            {
                Id = expense.Id,
                Category = expense.Category,
                Amount = expense.Amount,
                Description = expense.Description,
                ExpenseDate = expense.ExpenseDate,
                CreatedAt = expense.CreatedAt
            };
        }
    }
}
