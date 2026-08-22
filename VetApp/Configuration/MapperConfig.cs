using AutoMapper;
using VetApp.DTO;
using VetApp.Models;

namespace VetApp.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // ========== USER ==========
            CreateMap<User, UserReadOnlyDTO>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.Role.Name));

            // ========== VETERINARIAN ==========
            CreateMap<VeterinarianSignupDTO, User>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId!.Value));

            CreateMap<VeterinarianSignupDTO, Veterinarian>();

            CreateMap<Veterinarian, VeterinarianReadOnlyDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.User.Firstname))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.User.Lastname))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.User.Role.Name));

            CreateMap<VeterinarianUpdateDTO, Veterinarian>();

            // The DTO's Id is the VETERINARIAN id — it must NOT overwrite the
            // User's Id (which is the USER id, a different value). Without this
            // ignore, EF sees the PK as modified and refuses to save.
            CreateMap<VeterinarianUpdateDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // ========== OWNER ==========
            CreateMap<OwnerSignupDTO, User>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId!.Value));

            CreateMap<OwnerSignupDTO, Owner>();

            CreateMap<Owner, OwnerReadOnlyDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.User.Firstname))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.User.Lastname))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.User.Role.Name));

            CreateMap<OwnerUpdateDTO, Owner>();

            // Same story as Veterinarian above — DTO.Id is the OWNER id, must
            // not be mapped onto User.Id.
            CreateMap<OwnerUpdateDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // ========== PATIENT ==========
            CreateMap<PatientInsertDTO, Patient>();

            CreateMap<Patient, PatientReadOnlyDTO>()
                .ForMember(dest => dest.VeterinarianFullName,
                    opt => opt.MapFrom(src => src.Veterinarian.User.Firstname + " " + src.Veterinarian.User.Lastname))
                .ForMember(dest => dest.VeterinarianClinic,
                    opt => opt.MapFrom(src => src.Veterinarian.Clinic))
                .ForMember(dest => dest.OwnerFullName,
                    opt => opt.MapFrom(src => src.Owner.User.Firstname + " " + src.Owner.User.Lastname))
                .ForMember(dest => dest.OwnerPhoneNumber,
                    opt => opt.MapFrom(src => src.Owner.PhoneNumber));

            CreateMap<PatientUpdateDTO, Patient>();
        }
    }
}