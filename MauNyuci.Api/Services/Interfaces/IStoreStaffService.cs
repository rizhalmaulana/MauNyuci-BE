using MauNyuci.Api.DTOs.StoreStaff;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IStoreStaffService
    {
        Task<StoreStaffResponseDto> AddStaffAsync(Guid storeId, Guid ownerId, StoreStaffCreateDto request);
        Task<IEnumerable<StoreStaffResponseDto>> GetStoreStaffsAsync(Guid storeId, Guid ownerId);
        Task<StoreStaffResponseDto> UpdateStaffRoleAsync(Guid storeId, Guid ownerId, Guid staffId, StoreStaffUpdateDto request);
        Task RemoveStaffAsync(Guid storeId, Guid ownerId, Guid staffId);
    }
}
