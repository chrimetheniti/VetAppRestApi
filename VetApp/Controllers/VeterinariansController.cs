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
    [Route("api/v1/veterinarians")]
    public class VeterinariansController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public VeterinariansController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

       
        /// Gets a veterinarian by ID.
        
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

        
        /// Gets a paginated list of veterinarians with optional filtering.
        
        [HttpGet]
        [Authorize(Policy = "VIEW_VETERINARIANS")]
        [ProducesResponseType(typeof(PaginatedResult<UserReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<UserReadOnlyDTO>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] VeterinarianFiltersDTO? filters = null)
        {
            var result = await _applicationService.VeterinarianService
                .GetPaginatedVeterinariansAsync(pageNumber, pageSize, filters ?? new VeterinarianFiltersDTO());

            return Ok(result);
        }

       
        /// Updates an existing veterinarian.
       
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

       
        /// Deletes a veterinarian by ID.
       
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
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Vets can view their own profile
            if (currentUserRole == "VETERINARIAN" && IsOwnProfile(targetId))
            {
                return;
            }

            // Anyone with VIEW_VETERINARIANS capability can view
            if (User.HasClaim("capability", "VIEW_VETERINARIANS"))
            {
                return;
            }

            throw new EntityForbiddenException("Veterinarian",
                "You do not have permission to view this veterinarian.");
        }

        private void EnsureCanEditVeterinarian(int targetId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Vets can edit their own profile
            if (currentUserRole == "VETERINARIAN" && IsOwnProfile(targetId))
            {
                return;
            }

            // Admin can edit anyone
            if (User.HasClaim("capability", "EDIT_VETERINARIAN"))
            {
                return;
            }

            throw new EntityForbiddenException("Veterinarian",
                "You do not have permission to edit this veterinarian.");
        }

        private bool IsOwnProfile(int targetVetId)
        {
            // Note: targetVetId is the Veterinarian.Id, not User.Id
            // For accurate own-profile check, we'd need to load the vet and compare UserId
            // Simplified version: assume admin will use VIEW_VETERINARIANS for general access
            return false; // TODO: improve when we add ClaimsService helper
        }
    }
}
