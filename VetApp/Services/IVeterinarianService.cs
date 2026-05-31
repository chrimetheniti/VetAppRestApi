using VetApp.Core;
using VetApp.Core.Filters;
using VetApp.DTO;

namespace VetApp.Services
{
    public interface IVeterinarianService
    {
        Task<UserReadOnlyDTO> SignUpUserAsync(VeterinarianSignupDTO request);
        Task<VeterinarianReadOnlyDTO> GetByIdAsync(int id);
        Task<VeterinarianReadOnlyDTO> UpdateAsync(VeterinarianUpdateDTO request);
        Task<bool> DeleteAsync(int id);
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedVeterinariansAsync(int pageNumber, int pageSize,
            VeterinarianFiltersDTO filters);
    }
}
