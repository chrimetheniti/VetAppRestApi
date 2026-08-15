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
        /// Gets a paginated list of patients with optional filtering.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "VIEW_PATIENTS")]
        [ProducesResponseType(typeof(PaginatedResult<PatientReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<PatientReadOnlyDTO>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] PatientFiltersDTO? filters = null)
        {
            var result = await _applicationService.PatientService
                .GetPaginatedPatientsAsync(pageNumber, pageSize, filters ?? new PatientFiltersDTO());

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
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "EDIT_PATIENT")]
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
    }
}