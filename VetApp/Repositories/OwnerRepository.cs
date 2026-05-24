using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VetApp.Core;
using VetApp.Data;
using VetApp.Models;

namespace VetApp.Repositories
{
    public class OwnerRepository : BaseRepository<Owner>, IOwnerRepository
    {
        public OwnerRepository(VetAppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserOwnerByUsernameAsync(string username)
        {
            var userOwner = await _context.Users
                .Include(u => u.Owner)
                .Where(u => u.Username == username && u.Owner != null)
                .SingleOrDefaultAsync();

            return userOwner;
        }

        public async Task<PaginatedResult<User>> GetPaginatedOwnersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates)
        {
            int totalRecords;
            IQueryable<User> query = _context.Users
                .Include(u => u.Owner)
                .Where(u => u.Owner != null);

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

        public async Task<List<Patient>> GetOwnerPatientsAsync(int ownerId)
        {
            List<Patient> patients;

            patients = await _context.Patients
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();

            return patients;
        }
    }
}
