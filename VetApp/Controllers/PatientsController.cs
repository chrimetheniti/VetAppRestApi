using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetApp.Core;
using VetApp.Core.Filters;
using VetApp.DTO;
using VetApp.Exceptions;
using VetApp.Security;
using VetApp.Services;

namespace VetApp.Controllers
{
    [ApiController]
    [Route("api/v1/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IClaimsService _claimsService;

        public PatientsController(
            IApplicationService applicationService,
            IClaimsService claimsService)
        {
            _applicationService = applicationService;
            _claimsService = claimsService;
        }

        /// <summary>
        /// Gets a patient by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(PatientReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientReadOnlyDTO>> GetById(int id)
        {
            var patient = await _applicationService.PatientService.GetByIdAsync(id);
            EnsureCanViewPatient(patient);
            return Ok(patient);
        }

        /// <summary>
        /// Gets a paginated list of patients.
        /// ADMIN and RECEPTIONIST see all patients.
        /// VETERINARIAN sees only their own patients (filtered by VeterinarianId).
        /// OWNER sees only their own pets (filtered by OwnerId).
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedResult<PatientReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<PatientReadOnlyDTO>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] PatientFiltersDTO? filters = null)
        {
            filters ??= new PatientFiltersDTO();
            EnforceRoleBasedFilter(filters);

            var result = await _applicationService.PatientService
                .GetPaginatedPatientsAsync(pageNumber, pageSize, filters);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new patient.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "INSERT_PATIENT")]
        [ProducesResponseType(typeof(PatientReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PatientReadOnlyDTO>> Create(
            [FromBody] PatientInsertDTO request)
        {
            var created = await _applicationService.PatientService.InsertPatientAsync(request);
            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = created.Id },
                value: created);
        }

        /// <summary>
        /// Updates an existing patient.
        /// ADMIN and RECEPTIONIST (with EDIT_PATIENT capability) can edit anything.
        /// OWNER can edit their own pet's details but CANNOT reassign the vet
        /// or the owner — those two IDs are forced back to their existing values.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(PatientReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientReadOnlyDTO>> Update(
            int id,
            [FromBody] PatientUpdateDTO request)
        {
            if (id != request.Id)
            {
                throw new InvalidArgumentException("Patient",
                    "Id in URL does not match id in body.");
            }

            // Fetch the current record so we can (a) check ownership and
            // (b) block reassignment attempts by owners.
            var existing = await _applicationService.PatientService.GetByIdAsync(id);
            EnsureCanEditPatient(existing);

            // Owners cannot reassign their pet to a different vet or a
            // different owner. Force the sensitive fields back to the
            // current values regardless of what the client sent.
            if (_claimsService.GetCurrentUserRole() == "OWNER")
            {
                request.VeterinarianId = existing.VeterinarianId;
                request.OwnerId = existing.OwnerId;
            }

            var updated = await _applicationService.PatientService.UpdateAsync(request);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a patient by ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "DELETE_PATIENT")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _applicationService.PatientService.DeleteAsync(id);
            return NoContent();
        }

        // ===== Authorization helpers =====

        /// <summary>
        /// For VETERINARIAN and OWNER roles, forces the corresponding filter
        /// so they can only ever see their own patients/pets — even if the
        /// client tries to send different filter values.
        /// ADMIN and RECEPTIONIST: no forced filter, they see all patients
        /// (or apply the filters they explicitly requested).
        /// </summary>
        private void EnforceRoleBasedFilter(PatientFiltersDTO filters)
        {
            var currentUserRole = _claimsService.GetCurrentUserRole();

            if (currentUserRole == "VETERINARIAN")
            {
                var vetId = _claimsService.GetCurrentVeterinarianId();
                if (!vetId.HasValue)
                {
                    throw new EntityForbiddenException("Patient",
                        "Veterinarian ID not found in token.");
                }
                filters.VeterinarianId = vetId.Value;
            }
            else if (currentUserRole == "OWNER")
            {
                var ownerId = _claimsService.GetCurrentOwnerId();
                if (!ownerId.HasValue)
                {
                    throw new EntityForbiddenException("Patient",
                        "Owner ID not found in token.");
                }
                filters.OwnerId = ownerId.Value;
            }
            // ADMIN and RECEPTIONIST: use filters as-is (or empty = all patients)
        }

        private void EnsureCanViewPatient(PatientReadOnlyDTO patient)
        {
            // Admin / Receptionist: can view all patients
            if (_claimsService.HasCapability("VIEW_PATIENTS"))
            {
                return;
            }

            var currentUserRole = _claimsService.GetCurrentUserRole();

            // Veterinarian: can view own patients only
            if (currentUserRole == "VETERINARIAN")
            {
                var currentVetId = _claimsService.GetCurrentVeterinarianId();
                if (currentVetId.HasValue && currentVetId.Value == patient.VeterinarianId)
                {
                    return;
                }
            }

            // Owner: can view own pets only
            if (currentUserRole == "OWNER")
            {
                var currentOwnerId = _claimsService.GetCurrentOwnerId();
                if (currentOwnerId.HasValue && currentOwnerId.Value == patient.OwnerId)
                {
                    return;
                }
            }

            throw new EntityForbiddenException("Patient",
                "You do not have permission to view this patient.");
        }

        private void EnsureCanEditPatient(PatientReadOnlyDTO patient)
        {
            // Admin / Receptionist: can edit all patients (via EDIT_PATIENT capability).
            if (_claimsService.HasCapability("EDIT_PATIENT"))
            {
                return;
            }

            // Owner: can edit own pets only (but not reassign — that's handled in Update).
            if (_claimsService.GetCurrentUserRole() == "OWNER")
            {
                var currentOwnerId = _claimsService.GetCurrentOwnerId();
                if (currentOwnerId.HasValue && currentOwnerId.Value == patient.OwnerId)
                {
                    return;
                }
            }

            // Vets do NOT get to edit patient master data — that's a clinic-admin task.
            throw new EntityForbiddenException("Patient",
                "You do not have permission to edit this patient.");
        }
    }
}