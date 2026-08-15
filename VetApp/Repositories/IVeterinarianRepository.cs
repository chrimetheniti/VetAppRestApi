using VetApp.Core;
using VetApp.Models;
using System.Linq.Expressions;

namespace VetApp.Repositories
{
    public interface IVeterinarianRepository : IBaseRepository<Veterinarian>
    {
        Task<Veterinarian?> GetByIdWithUserAsync(int id);
        Task<List<Patient>> GetVeterinarianPatientsAsync(int veterinarianId);
        Task<User?> GetUserVeterinarianByUsernameAsync(string username);
        Task<PaginatedResult<User>> GetPaginatedVeterinariansAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates);
    }
}