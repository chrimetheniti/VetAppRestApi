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
    public class OwnerService : IOwnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly ILogger<OwnerService> _logger;

        public OwnerService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<OwnerService> logger, IEncryptionUtil encryptionUtil)
        {
            _encryptionUtil = encryptionUtil;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserReadOnlyDTO> SignUpUserAsync(OwnerSignupDTO request)
        {
            var owner = _mapper.Map<Owner>(request);
            var user = _mapper.Map<User>(request);

            var existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new EntityAlreadyExistsException("User",
                    $"User with username {existingUser.Username} already exists");
            }

            user.Owner = owner;
            user.Password = _encryptionUtil.Encrypt(user.Password);

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Owner {Username} signed up successfully.", user.Username);
            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<OwnerReadOnlyDTO> GetByIdAsync(int id)
        {
            var owner = await _unitOfWork.OwnerRepository.GetByIdAsync(id);
            if (owner == null)
            {
                throw new EntityNotFoundException("Owner", $"Owner with id {id} not found");
            }

            _logger.LogInformation("Owner with id {Id} found", id);
            return _mapper.Map<OwnerReadOnlyDTO>(owner);
        }

        public async Task<OwnerReadOnlyDTO> UpdateAsync(OwnerUpdateDTO request)
        {
            var owner = await _unitOfWork.OwnerRepository.GetByIdAsync(request.Id);
            if (owner == null)
            {
                throw new EntityNotFoundException("Owner",
                    $"Owner with id {request.Id} not found");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(owner.UserId);
            if (user == null)
            {
                throw new EntityNotFoundException("User",
                    $"User for owner id {request.Id} not found");
            }

            _mapper.Map(request, owner);
            _mapper.Map(request, user);

            await _unitOfWork.OwnerRepository.UpdateAsync(owner);
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Owner {Id} updated successfully.", request.Id);
            return _mapper.Map<OwnerReadOnlyDTO>(owner);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var owner = await _unitOfWork.OwnerRepository.GetByIdAsync(id);
            if (owner == null)
            {
                throw new EntityNotFoundException("Owner", $"Owner with id {id} not found");
            }

            await _unitOfWork.OwnerRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Owner {Id} deleted successfully.", id);
            return true;
        }

        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedOwnersAsync(
            int pageNumber, int pageSize, OwnerFiltersDTO filters)
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
            if (!string.IsNullOrEmpty(filters.PhoneNumber))
            {
                predicates.Add(u => u.Owner!.PhoneNumber!.Contains(filters.PhoneNumber));
            }

            var result = await _unitOfWork.OwnerRepository
                .GetPaginatedOwnersAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = _mapper.Map<List<UserReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

            _logger.LogInformation("Retrieved {Count} owners", dtoResult.Data.Count);
            return dtoResult;
        }
    }
}
