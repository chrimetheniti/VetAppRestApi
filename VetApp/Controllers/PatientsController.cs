using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VetApp.Core;
using VetApp.Core.Filters;
using VetApp.DTO;
using VetApp.Exceptions;
using VetApp.Services;

namespace VetApp.Controllers
{
    [ApiController]
    [Route("api/v1/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public PatientsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        
        /// Gets a patient by ID.
      
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

       
        /// Gets a paginated list of patients with optional filtering.
       
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

        
        /// Creates a new patient.
       
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

       
        /// Updates an existing patient.
       
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

        
        /// Deletes a patient by ID.
       
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
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Admin / Receptionist: can view all patients
            if (User.HasClaim("capability", "VIEW_PATIENTS"))
            {
                return;
            }

            // Veterinarian: can view own patients only
            if (currentUserRole == "VETERINARIAN")
            {
                // TODO: needs ClaimsService to map User.Id → Veterinarian.Id
                // For now, allow vets to view any patient (will tighten later)
                return;
            }

            // Owner: can view own pets only
            if (currentUserRole == "OWNER")
            {
                // TODO: same as above for Owner
                return;
            }

            throw new EntityForbiddenException("Patient",
                "You do not have permission to view this patient.");
        }
    }
}