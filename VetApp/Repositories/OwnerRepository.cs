using Microsoft.EntityFrameworkCore;
using VetApp.Core;
using VetApp.Data;
using VetApp.Models;
using System.Linq.Expressions;

namespace VetApp.Repositories
{
    public class OwnerRepository : BaseRepository<Owner>, IOwnerRepository
    {
        public OwnerRepository(VetAppDbContext context) : base(context)
        {
        }

        public async Task<Owner?> GetByIdWithUserAsync(int id)
        {
            return await _context.Owners
                .Include(o => o.User)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<User?> GetUserOwnerByUsernameAsync(string username)
        {
            var userOwner = await _context.Users
                .Include(u => u.Owner)
                .Where(u => u.Username == username && u.Owner != null)
                .SingleOrDefaultAsync();

            return userOwner;
        }

        public async Task<PaginatedResult<Owner>> GetPaginatedOwnersAsync(int pageNumber, int pageSize,
            List<Expression<Func<Owner, bool>>> predicates)
        {
            IQueryable<Owner> query = _context.Owners
                .Include(o => o.User)
                    .ThenInclude(u => u.Role);

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            int totalRecords = await query.CountAsync();
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(o => o.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Owner>()
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