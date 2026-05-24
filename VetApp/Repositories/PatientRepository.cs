using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VetApp.Core;
using VetApp.Data;
using VetApp.Models;

namespace VetApp.Repositories
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        public PatientRepository(VetAppDbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetByChipNumberAsync(string? chipNumber)
        {
            return await _context.Patients
                .Where(p => p.ChipNumber == chipNumber)
                .SingleOrDefaultAsync();
        }

        public async Task<Patient?> GetPatientWithDetailsAsync(int patientId)
        {
            return await _context.Patients
                .Include(p => p.Veterinarian)
                    .ThenInclude(v => v.User)
                .Include(p => p.Owner)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<PaginatedResult<Patient>> GetPaginatedPatientsAsync(int pageNumber, int pageSize,
            List<Expression<Func<Patient, bool>>> predicates)
        {
            IQueryable<Patient> query = _context.Patients;

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
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Patient>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
