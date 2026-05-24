using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VetApp.Core;
using VetApp.Data;
using VetApp.Models;

namespace VetApp.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(VetAppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);

        public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates)
        {
            int totalRecords;
            IQueryable<User> query = _context.Users;

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            totalRecords = await query.CountAsync();
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<User>()
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
