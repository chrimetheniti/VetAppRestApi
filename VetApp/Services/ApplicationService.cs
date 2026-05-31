namespace VetApp.Services
{
    public class ApplicationService : IApplicationService
    {
        public IUserService UserService { get; }
        public IVeterinarianService VeterinarianService { get; }
        public IPatientService PatientService { get; }
        public IOwnerService OwnerService { get; }

        public ApplicationService(
            IUserService userService,
            IVeterinarianService veterinarianService,
            IPatientService patientService,
            IOwnerService ownerService)
        {
            UserService = userService;
            VeterinarianService = veterinarianService;
            PatientService = patientService;
            OwnerService = ownerService;
        }
    }
}
