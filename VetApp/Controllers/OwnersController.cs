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
    [Route("api/v1/owners")]
    public class OwnersController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public OwnersController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        
        /// Gets an owner by ID.
        
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(OwnerReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OwnerReadOnlyDTO>> GetById(int id)
        {
            EnsureCanViewOwner(id);
            var owner = await _applicationService.OwnerService.GetByIdAsync(id);
            return Ok(owner);
        }

        
        /// Gets a paginated list of owners with optional filtering.
        
        [HttpGet]
        [Authorize(Policy = "VIEW_OWNERS")]
        [ProducesResponseType(typeof(PaginatedResult<UserReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<UserReadOnlyDTO>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OwnerFiltersDTO? filters = null)
        {
            var result = await _applicationService.OwnerService
                .GetPaginatedOwnersAsync(pageNumber, pageSize, filters ?? new OwnerFiltersDTO());

            return Ok(result);
        }

        
        /// Updates an existing owner.
        
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(OwnerReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OwnerReadOnlyDTO>> Update(
            int id,
            [FromBody] OwnerUpdateDTO request)
        {
            if (id != request.Id)
            {
                throw new InvalidArgumentException("Owner",
                    "Id in URL does not match id in body.");
            }

            EnsureCanEditOwner(id);

            var updated = await _applicationService.OwnerService.UpdateAsync(request);
            return Ok(updated);
        }

        
        /// Deletes an owner by ID.
       
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "DELETE_OWNER")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _applicationService.OwnerService.DeleteAsync(id);
            return NoContent();
        }

        // ===== Authorization helpers =====

        private void EnsureCanViewOwner(int targetId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Anyone with VIEW_OWNERS capability can view (Admin, Receptionist)
            if (User.HasClaim("capability", "VIEW_OWNERS"))
            {
                return;
            }

            // Owner: can view own profile only
            if (currentUserRole == "OWNER" && IsOwnProfile(targetId))
            {
                return;
            }

            throw new EntityForbiddenException("Owner",
                "You do not have permission to view this owner.");
        }

        private void EnsureCanEditOwner(int targetId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Admin can edit anyone
            if (User.HasClaim("capability", "EDIT_OWNER"))
            {
                return;
            }

            // Owner: can edit own profile
            if (currentUserRole == "OWNER" && IsOwnProfile(targetId))
            {
                return;
            }

            throw new EntityForbiddenException("Owner",
                "You do not have permission to edit this owner.");
        }

        private bool IsOwnProfile(int targetOwnerId)
        {
            // Note: targetOwnerId is the Owner.Id, not User.Id
            // TODO: improve when we add ClaimsService helper
            return false;
        }
    }
}
