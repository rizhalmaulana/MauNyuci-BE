using MauNyuci.Api.DTOs.Store;
using MauNyuci.Api.Models;
using MauNyuci.Api.Repositories.Interfaces;
using MauNyuci.Api.Services.Interfaces;

namespace MauNyuci.Api.Services.Implementations
{
    public class StoreBankAccountService : IStoreBankAccountService
    {
        private readonly IStoreBankAccountRepository _bankAccountRepository;
        private readonly IStoreRepository _storeRepository;

        public StoreBankAccountService(IStoreBankAccountRepository bankAccountRepository, IStoreRepository storeRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _storeRepository = storeRepository;
        }

        public async Task<IEnumerable<StoreBankAccountResponseDto>> GetStoreBankAccountsAsync(Guid storeId)
        {
            var accounts = await _bankAccountRepository.GetByStoreIdAsync(storeId);
            return accounts.Where(a => a.IsActive).Select(MapToDto);
        }

        public async Task<IEnumerable<StoreBankAccountResponseDto>> GetMyBankAccountsAsync(Guid userId)
        {
            var store = await _storeRepository.GetByOwnerOrStaffIdAsync(userId);
            if (store == null) throw new Exception("Toko tidak ditemukan.");

            var accounts = await _bankAccountRepository.GetByStoreIdAsync(store.Id);
            return accounts.Select(MapToDto);
        }

        public async Task<StoreBankAccountResponseDto> AddBankAccountAsync(Guid userId, StoreBankAccountCreateDto request)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(userId);
            if (store == null) throw new Exception("Toko tidak ditemukan.");

            var newAccount = new StoreBankAccount
            {
                StoreId = store.Id,
                BankName = request.BankName,
                AccountNumber = request.AccountNumber,
                AccountHolderName = request.AccountHolderName,
                IsActive = true,
                QrisImageUrl = request.QrisImageUrl
            };

            var created = await _bankAccountRepository.CreateAsync(newAccount);
            return MapToDto(created);
        }

        public async Task<StoreBankAccountResponseDto> UpdateBankAccountAsync(Guid userId, Guid bankAccountId, StoreBankAccountUpdateDto request)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(userId);
            if (store == null) throw new Exception("Toko tidak ditemukan.");

            var account = await _bankAccountRepository.GetByIdAsync(bankAccountId);
            if (account == null || account.StoreId != store.Id)
            {
                throw new Exception("Rekening tidak ditemukan atau bukan milik Anda.");
            }

            if (request.BankName != null) account.BankName = request.BankName;
            if (request.AccountNumber != null) account.AccountNumber = request.AccountNumber;
            if (request.AccountHolderName != null) account.AccountHolderName = request.AccountHolderName;
            if (request.IsActive.HasValue) account.IsActive = request.IsActive.Value;
            if (request.QrisImageUrl != null) account.QrisImageUrl = request.QrisImageUrl;

            var updated = await _bankAccountRepository.UpdateAsync(account);
            return MapToDto(updated);
        }

        public async Task DeleteBankAccountAsync(Guid userId, Guid bankAccountId)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(userId);
            if (store == null) throw new Exception("Toko tidak ditemukan.");

            var account = await _bankAccountRepository.GetByIdAsync(bankAccountId);
            if (account == null || account.StoreId != store.Id)
            {
                throw new Exception("Rekening tidak ditemukan atau bukan milik Anda.");
            }

            await _bankAccountRepository.DeleteAsync(account);
        }

        private StoreBankAccountResponseDto MapToDto(StoreBankAccount account)
        {
            return new StoreBankAccountResponseDto
            {
                Id = account.Id,
                BankName = account.BankName,
                AccountNumber = account.AccountNumber,
                AccountHolderName = account.AccountHolderName,
                IsActive = account.IsActive,
                QrisImageUrl = account.QrisImageUrl
            };
        }
    }
}
