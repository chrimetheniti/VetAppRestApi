using System.Linq.Expressions;
using VetApp.Core;
using VetApp.Models;

namespace VetApp.Repositories
{
    public interface IPatientRepository : IBaseRepository<Patient>
    {
        Task<Patient?> GetByChipNumberAsync(string? chipNumber);
        Task<Patient?> GetPatientWithDetailsAsync(int patientId);
        Task<PaginatedResult<Patient>> GetPaginatedPatientsAsync(int pageNumber, int pageSize,
            List<Expression<Func<Patient, bool>>> predicates);
    }
}
