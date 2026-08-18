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
    [Route("api/v1/veterinarians")]
    public class VeterinariansController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IClaimsService _claimsService;

        public VeterinariansController(
            IApplicationService applicationService,
            IClaimsService claimsService)
        {
            _applicationService = applicationService;
            _claimsService = claimsService;
        }

        /// <summary>
        /// Gets a veterinarian by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(VeterinarianReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VeterinarianReadOnlyDTO>> GetById(int id)
        {
            EnsureCanViewVeterinarian(id);
            var vet = await _applicationService.VeterinarianService.GetByIdAsync(id);
            return Ok(vet);
        }

        /// <summary>
        /// Gets a paginated list of veterinarians with optional filtering.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "VIEW_VETERINARIANS")]
        [ProducesResponseType(typeof(PaginatedResult<VeterinarianReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<VeterinarianReadOnlyDTO>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] VeterinarianFiltersDTO? filters = null)
        {
            var result = await _applicationService.VeterinarianService
                .GetPaginatedVeterinariansAsync(pageNumber, pageSize, filters ?? new VeterinarianFiltersDTO());

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing veterinarian.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(VeterinarianReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VeterinarianReadOnlyDTO>> Update(
            int id,
            [FromBody] VeterinarianUpdateDTO request)
        {
            if (id != request.Id)
            {
                throw new InvalidArgumentException("Veterinarian",
                    "Id in URL does not match id in body.");
            }

            EnsureCanEditVeterinarian(id);

            var updated = await _applicationService.VeterinarianService.UpdateAsync(request);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a veterinarian by ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "DELETE_VETERINARIAN")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _applicationService.VeterinarianService.DeleteAsync(id);
            return NoContent();
        }

        // ===== Authorization helpers =====

        private void EnsureCanViewVeterinarian(int targetId)
        {
            // Anyone with VIEW_VETERINARIANS capability can view (Admin, Receptionist)
            if (_claimsService.HasCapability("VIEW_VETERINARIANS"))
            {
                return;
            }

            // Vets can view their own profile
            var currentUserRole = _claimsService.GetCurrentUserRole();
            if (currentUserRole == "VETERINARIAN" && IsOwnProfile(targetId))
            {
                return;
            }

            throw new EntityForbiddenException("Veterinarian",
                "You do not have permission to view this veterinarian.");
        }

        private void EnsureCanEditVeterinarian(int targetId)
        {
            // Anyone with EDIT_VETERINARIAN capability can edit (Admin)
            if (_claimsService.HasCapability("EDIT_VETERINARIAN"))
            {
                return;
            }

            // Vets can edit their own profile
            var currentUserRole = _claimsService.GetCurrentUserRole();
            if (currentUserRole == "VETERINARIAN" && IsOwnProfile(targetId))
            {
                return;
            }

            throw new EntityForbiddenException("Veterinarian",
                "You do not have permission to edit this veterinarian.");
        }

        private bool IsOwnProfile(int targetVetId)
        {
            var currentVetId = _claimsService.GetCurrentVeterinarianId();
            return currentVetId.HasValue && currentVetId.Value == targetVetId;
        }
    }
}