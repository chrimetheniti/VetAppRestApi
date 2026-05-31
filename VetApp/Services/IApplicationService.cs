namespace VetApp.Services
{
    public interface IApplicationService
    {
        IUserService UserService { get; }
        IVeterinarianService VeterinarianService { get; }
        IPatientService PatientService { get; }
        IOwnerService OwnerService { get; }
    }

}
