using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VetApp.Core;
using VetApp.Data;
using VetApp.Models;

namespace VetApp.Repositories
{
    public class VeterinarianRepository : BaseRepository<Veterinarian>, IVeterinarianRepository
    {
        public VeterinarianRepository(VetAppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserVeterinarianByUsernameAsync(string username)
        {
            var userVeterinarian = await _context.Users
                .Include(u => u.Veterinarian)
                .Where(u => u.Username == username && u.Veterinarian != null)
                .SingleOrDefaultAsync();

            return userVeterinarian;
        }

        public async Task<PaginatedResult<User>> GetPaginatedVeterinariansAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates)
        {
            int totalRecords;
            IQueryable<User> query = _context.Users
                .Include(u => u.Veterinarian)
                .Where(u => u.Veterinarian != null);

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

        public async Task<List<Patient>> GetVeterinarianPatientsAsync(int veterinarianId)
        {
            List<Patient> patients;

            patients = await _context.Patients
                .Where(p => p.VeterinarianId == veterinarianId)
                .ToListAsync();

            return patients;
        }
    }
}
