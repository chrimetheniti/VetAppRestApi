using VetApp.Core;
using VetApp.Models;
using System.Linq.Expressions;

namespace VetApp.Repositories
{
    public interface IOwnerRepository : IBaseRepository<Owner>
    {
        Task<Owner?> GetByIdWithUserAsync(int id);
        Task<List<Patient>> GetOwnerPatientsAsync(int ownerId);
        Task<User?> GetUserOwnerByUsernameAsync(string username);
        Task<PaginatedResult<User>> GetPaginatedOwnersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates);
    }
}