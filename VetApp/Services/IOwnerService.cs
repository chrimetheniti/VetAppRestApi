using VetApp.Core;
using VetApp.Core.Filters;
using VetApp.DTO;

namespace VetApp.Services
{
    public interface IOwnerService
    {
        Task<UserReadOnlyDTO> SignUpUserAsync(OwnerSignupDTO request);
        Task<OwnerReadOnlyDTO> GetByIdAsync(int id);
        Task<OwnerReadOnlyDTO> UpdateAsync(OwnerUpdateDTO request);
        Task<bool> DeleteAsync(int id);
        Task<PaginatedResult<OwnerReadOnlyDTO>> GetPaginatedOwnersAsync(int pageNumber, int pageSize,
            OwnerFiltersDTO filters);
    }
}