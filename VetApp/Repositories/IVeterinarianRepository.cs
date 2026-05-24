using System.Linq.Expressions;
using VetApp.Core;
using VetApp.Models;

namespace VetApp.Repositories
{
    public interface IVeterinarianRepository : IBaseRepository<Veterinarian>
    {
        Task<List<Patient>> GetVeterinarianPatientsAsync(int veterinarianId);
        Task<User?> GetUserVeterinarianByUsernameAsync(string username);
        Task<PaginatedResult<User>> GetPaginatedVeterinariansAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates);
    }
}
