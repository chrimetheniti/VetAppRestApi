using VetApp.Data;

namespace VetApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VetAppDbContext _context;

        public IUserRepository UserRepository { get; }
        public IVeterinarianRepository VeterinarianRepository { get; }
        public IPatientRepository PatientRepository { get; }
        public IOwnerRepository OwnerRepository { get; }

        public UnitOfWork(VetAppDbContext context)
        {
            _context = context;
            UserRepository = new UserRepository(context);
            VeterinarianRepository = new VeterinarianRepository(context);
            PatientRepository = new PatientRepository(context);
            OwnerRepository = new OwnerRepository(context);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
