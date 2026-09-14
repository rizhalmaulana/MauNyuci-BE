using MauNyuci.Api.Models;

namespace MauNyuci.Api.Repositories.Interfaces
{
    public interface IStoreBankAccountRepository
    {
        Task<StoreBankAccount?> GetByIdAsync(Guid id);
        Task<IEnumerable<StoreBankAccount>> GetByStoreIdAsync(Guid storeId);
        Task<StoreBankAccount> CreateAsync(StoreBankAccount bankAccount);
        Task<StoreBankAccount> UpdateAsync(StoreBankAccount bankAccount);
        Task DeleteAsync(StoreBankAccount bankAccount);
    }
}
