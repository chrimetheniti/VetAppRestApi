using System.Linq.Expressions;
using VetApp.Core;
using VetApp.Models;

namespace VetApp.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates);
    }
}
