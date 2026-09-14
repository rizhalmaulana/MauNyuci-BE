using MauNyuci.Api.Data;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Repositories.Implementations
{
    public class StoreBankAccountRepository : IStoreBankAccountRepository
    {
        private readonly AppDbContext _context;

        public StoreBankAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StoreBankAccount?> GetByIdAsync(Guid id)
        {
            return await _context.StoreBankAccounts
                .Include(s => s.Store)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<StoreBankAccount>> GetByStoreIdAsync(Guid storeId)
        {
            return await _context.StoreBankAccounts
                .Where(s => s.StoreId == storeId)
                .ToListAsync();
        }

        public async Task<StoreBankAccount> CreateAsync(StoreBankAccount bankAccount)
        {
            await _context.StoreBankAccounts.AddAsync(bankAccount);
            await _context.SaveChangesAsync();
            return bankAccount;
        }

        public async Task<StoreBankAccount> UpdateAsync(StoreBankAccount bankAccount)
        {
            _context.StoreBankAccounts.Update(bankAccount);
            await _context.SaveChangesAsync();
            return bankAccount;
        }

        public async Task DeleteAsync(StoreBankAccount bankAccount)
        {
            _context.StoreBankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();
        }
    }
}
