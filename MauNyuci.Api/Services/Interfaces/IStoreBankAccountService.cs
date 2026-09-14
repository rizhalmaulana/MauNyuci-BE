using MauNyuci.Api.DTOs.Store;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IStoreBankAccountService
    {
        Task<IEnumerable<StoreBankAccountResponseDto>> GetStoreBankAccountsAsync(Guid storeId);
        Task<IEnumerable<StoreBankAccountResponseDto>> GetMyBankAccountsAsync(Guid userId);
        Task<StoreBankAccountResponseDto> AddBankAccountAsync(Guid userId, StoreBankAccountCreateDto request);
        Task<StoreBankAccountResponseDto> UpdateBankAccountAsync(Guid userId, Guid bankAccountId, StoreBankAccountUpdateDto request);
        Task DeleteBankAccountAsync(Guid userId, Guid bankAccountId);
    }
}
