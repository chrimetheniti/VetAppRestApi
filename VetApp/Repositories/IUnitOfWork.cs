namespace VetApp.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IVeterinarianRepository VeterinarianRepository { get; }
        IPatientRepository PatientRepository { get; }
        IOwnerRepository OwnerRepository { get; }
        Task<bool> SaveAsync();
    }
}
