using VetApp.Core;
using VetApp.Core.Filters;
using VetApp.DTO;

namespace VetApp.Services
{
    public interface IPatientService
    {
        Task<PatientReadOnlyDTO> InsertPatientAsync(PatientInsertDTO request);
        Task<PatientReadOnlyDTO> GetByIdAsync(int id);
        Task<PatientReadOnlyDTO> UpdateAsync(PatientUpdateDTO request);
        Task<bool> DeleteAsync(int id);
        Task<PaginatedResult<PatientReadOnlyDTO>> GetPaginatedPatientsAsync(int pageNumber, int pageSize,
            PatientFiltersDTO filters);
    }
}
