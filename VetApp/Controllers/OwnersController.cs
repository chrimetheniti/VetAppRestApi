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
    [Route("api/v1/owners")]
    public class OwnersController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IClaimsService _claimsService;

        public OwnersController(
            IApplicationService applicationService,
            IClaimsService claimsService)
        {
            _applicationService = applicationService;
            _claimsService = claimsService;
        }

        /// <summary>
        /// Gets an owner by ID.
        /// </summary>
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

        /// <summary>
        /// Gets a paginated list of owners with optional filtering.
        /// </summary>
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

        /// <summary>
        /// Updates an existing owner.
        /// </summary>
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

        /// <summary>
        /// Deletes an owner by ID.
        /// </summary>
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
            // Anyone with VIEW_OWNERS capability can view (Admin, Receptionist)
            if (_claimsService.HasCapability("VIEW_OWNERS"))
            {
                return;
            }

            // Owners can view their own profile
            var currentUserRole = _claimsService.GetCurrentUserRole();
            if (currentUserRole == "OWNER" && IsOwnProfile(targetId))
            {
                return;
            }

            throw new EntityForbiddenException("Owner",
                "You do not have permission to view this owner.");
        }

        private void EnsureCanEditOwner(int targetId)
        {
            // Anyone with EDIT_OWNER capability can edit (Admin)
            if (_claimsService.HasCapability("EDIT_OWNER"))
            {
                return;
            }

            // Owners can edit their own profile
            var currentUserRole = _claimsService.GetCurrentUserRole();
            if (currentUserRole == "OWNER" && IsOwnProfile(targetId))
            {
                return;
            }

            throw new EntityForbiddenException("Owner",
                "You do not have permission to edit this owner.");
        }

        private bool IsOwnProfile(int targetOwnerId)
        {
            var currentOwnerId = _claimsService.GetCurrentOwnerId();
            return currentOwnerId.HasValue && currentOwnerId.Value == targetOwnerId;
        }
    }
}