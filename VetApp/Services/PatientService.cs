using AutoMapper;
using System.Linq.Expressions;
using VetApp.Core;
using VetApp.Core.Filters;
using VetApp.DTO;
using VetApp.Exceptions;
using VetApp.Models;
using VetApp.Repositories;

namespace VetApp.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PatientService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PatientReadOnlyDTO> InsertPatientAsync(PatientInsertDTO request)
        {
            // Verify vet and owner exist
            var vet = await _unitOfWork.VeterinarianRepository.GetByIdAsync(request.VeterinarianId!.Value);
            if (vet == null)
            {
                throw new EntityNotFoundException("Veterinarian",
                    $"Veterinarian with id {request.VeterinarianId} not found");
            }

            var owner = await _unitOfWork.OwnerRepository.GetByIdAsync(request.OwnerId!.Value);
            if (owner == null)
            {
                throw new EntityNotFoundException("Owner",
                    $"Owner with id {request.OwnerId} not found");
            }

            // Check chip number uniqueness if provided
            if (!string.IsNullOrEmpty(request.ChipNumber))
            {
                var existingPatient = await _unitOfWork.PatientRepository.GetByChipNumberAsync(request.ChipNumber);
                if (existingPatient != null)
                {
                    throw new EntityAlreadyExistsException("Patient",
                        $"Patient with chip number {request.ChipNumber} already exists");
                }
            }

            var patient = _mapper.Map<Patient>(request);

            await _unitOfWork.PatientRepository.AddAsync(patient);
            await _unitOfWork.SaveAsync();

            // Re-fetch with details for proper DTO mapping
            var savedPatient = await _unitOfWork.PatientRepository.GetPatientWithDetailsAsync(patient.Id);

            _logger.LogInformation("Patient {Name} (id {Id}) inserted successfully.", patient.Name, patient.Id);
            return _mapper.Map<PatientReadOnlyDTO>(savedPatient);
        }

        public async Task<PatientReadOnlyDTO> GetByIdAsync(int id)
        {
            var patient = await _unitOfWork.PatientRepository.GetPatientWithDetailsAsync(id);
            if (patient == null)
            {
                throw new EntityNotFoundException("Patient", $"Patient with id {id} not found");
            }

            _logger.LogInformation("Patient with id {Id} found", id);
            return _mapper.Map<PatientReadOnlyDTO>(patient);
        }

        public async Task<PatientReadOnlyDTO> UpdateAsync(PatientUpdateDTO request)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.Id);
            if (patient == null)
            {
                throw new EntityNotFoundException("Patient",
                    $"Patient with id {request.Id} not found");
            }

            // Verify new vet and owner exist (in case they changed)
            var vet = await _unitOfWork.VeterinarianRepository.GetByIdAsync(request.VeterinarianId!.Value);
            if (vet == null)
            {
                throw new EntityNotFoundException("Veterinarian",
                    $"Veterinarian with id {request.VeterinarianId} not found");
            }

            var owner = await _unitOfWork.OwnerRepository.GetByIdAsync(request.OwnerId!.Value);
            if (owner == null)
            {
                throw new EntityNotFoundException("Owner",
                    $"Owner with id {request.OwnerId} not found");
            }

            // Check chip number uniqueness (only if changed)
            if (!string.IsNullOrEmpty(request.ChipNumber) && request.ChipNumber != patient.ChipNumber)
            {
                var existingPatient = await _unitOfWork.PatientRepository.GetByChipNumberAsync(request.ChipNumber);
                if (existingPatient != null && existingPatient.Id != request.Id)
                {
                    throw new EntityAlreadyExistsException("Patient",
                        $"Patient with chip number {request.ChipNumber} already exists");
                }
            }

            _mapper.Map(request, patient);

            await _unitOfWork.PatientRepository.UpdateAsync(patient);
            await _unitOfWork.SaveAsync();

            var updatedPatient = await _unitOfWork.PatientRepository.GetPatientWithDetailsAsync(patient.Id);

            _logger.LogInformation("Patient {Id} updated successfully.", request.Id);
            return _mapper.Map<PatientReadOnlyDTO>(updatedPatient);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                throw new EntityNotFoundException("Patient", $"Patient with id {id} not found");
            }

            await _unitOfWork.PatientRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Patient {Id} deleted successfully.", id);
            return true;
        }

        public async Task<PaginatedResult<PatientReadOnlyDTO>> GetPaginatedPatientsAsync(
            int pageNumber, int pageSize, PatientFiltersDTO filters)
        {
            List<Expression<Func<Patient, bool>>> predicates = [];

            if (!string.IsNullOrEmpty(filters.Name))
            {
                predicates.Add(p => p.Name.Contains(filters.Name));
            }
            if (!string.IsNullOrEmpty(filters.ChipNumber))
            {
                predicates.Add(p => p.ChipNumber == filters.ChipNumber);
            }
            if (!string.IsNullOrEmpty(filters.Species))
            {
                predicates.Add(p => p.Species == filters.Species);
            }
            if (!string.IsNullOrEmpty(filters.Breed))
            {
                predicates.Add(p => p.Breed == filters.Breed);
            }
            if (filters.VeterinarianId.HasValue)
            {
                predicates.Add(p => p.VeterinarianId == filters.VeterinarianId.Value);
            }
            if (filters.OwnerId.HasValue)
            {
                predicates.Add(p => p.OwnerId == filters.OwnerId.Value);
            }

            var result = await _unitOfWork.PatientRepository
                .GetPaginatedPatientsAsync(pageNumber, pageSize, predicates);

            var dtoResult = new PaginatedResult<PatientReadOnlyDTO>()
            {
                Data = _mapper.Map<List<PatientReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

            _logger.LogInformation("Retrieved {Count} patients", dtoResult.Data.Count);
            return dtoResult;
        }
    }
}
