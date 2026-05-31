using AutoMapper;
using System.Linq.Expressions;
using VetApp.Core;
using VetApp.Core.Filters;
using VetApp.DTO;
using VetApp.Exceptions;
using VetApp.Models;
using VetApp.Repositories;
using VetApp.Security;

namespace VetApp.Services
{
    public class VeterinarianService : IVeterinarianService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly ILogger<VeterinarianService> _logger;

        public VeterinarianService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<VeterinarianService> logger, IEncryptionUtil encryptionUtil)
        {
            _encryptionUtil = encryptionUtil;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserReadOnlyDTO> SignUpUserAsync(VeterinarianSignupDTO request)
        {
            var veterinarian = _mapper.Map<Veterinarian>(request);
            var user = _mapper.Map<User>(request);

            var existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new EntityAlreadyExistsException("User",
                    $"User with username {existingUser.Username} already exists");
            }

            user.Veterinarian = veterinarian;
            user.Password = _encryptionUtil.Encrypt(user.Password);

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Veterinarian {Username} signed up successfully.", user.Username);
            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<VeterinarianReadOnlyDTO> GetByIdAsync(int id)
        {
            var vet = await _unitOfWork.VeterinarianRepository.GetByIdAsync(id);
            if (vet == null)
            {
                throw new EntityNotFoundException("Veterinarian",
                    $"Veterinarian with id {id} not found");
            }

            _logger.LogInformation("Veterinarian with id {Id} found", id);
            return _mapper.Map<VeterinarianReadOnlyDTO>(vet);
        }

        public async Task<VeterinarianReadOnlyDTO> UpdateAsync(VeterinarianUpdateDTO request)
        {
            var vet = await _unitOfWork.VeterinarianRepository.GetByIdAsync(request.Id);
            if (vet == null)
            {
                throw new EntityNotFoundException("Veterinarian",
                    $"Veterinarian with id {request.Id} not found");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(vet.UserId);
            if (user == null)
            {
                throw new EntityNotFoundException("User",
                    $"User for veterinarian id {request.Id} not found");
            }

            // Map updated fields onto existing entities
            _mapper.Map(request, vet);
            _mapper.Map(request, user);

            await _unitOfWork.VeterinarianRepository.UpdateAsync(vet);
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Veterinarian {Id} updated successfully.", request.Id);
            return _mapper.Map<VeterinarianReadOnlyDTO>(vet);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vet = await _unitOfWork.VeterinarianRepository.GetByIdAsync(id);
            if (vet == null)
            {
                throw new EntityNotFoundException("Veterinarian",
                    $"Veterinarian with id {id} not found");
            }

            await _unitOfWork.VeterinarianRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Veterinarian {Id} deleted successfully.", id);
            return true;
        }

        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedVeterinariansAsync(
            int pageNumber, int pageSize, VeterinarianFiltersDTO filters)
        {
            List<Expression<Func<User, bool>>> predicates = [];

            if (!string.IsNullOrEmpty(filters.Username))
            {
                predicates.Add(u => u.Username == filters.Username);
            }
            if (!string.IsNullOrEmpty(filters.Email))
            {
                predicates.Add(u => u.Email == filters.Email);
            }
            if (!string.IsNullOrEmpty(filters.Clinic))
            {
                predicates.Add(u => u.Veterinarian!.Clinic.Contains(filters.Clinic));
            }

            var result = await _unitOfWork.VeterinarianRepository
                .GetPaginatedVeterinariansAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = _mapper.Map<List<UserReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

            _logger.LogInformation("Retrieved {Count} veterinarians", dtoResult.Data.Count);
            return dtoResult;
        }
    }
}
