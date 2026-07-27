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
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public UsersController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

       
        /// Gets a user by their ID.
       
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserById(int id)
        {
            EnsureCanViewUser(id);
            var user = await _applicationService.UserService.GetUserByIdAsync(id);
            return Ok(user);
        }

     
        /// Gets a user by their username.
        
        [HttpGet("by-username/{username}")]
        [Authorize]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserByUsername(string username)
        {
            EnsureCanViewUser(username);
            var user = await _applicationService.UserService.GetUserByUsernameAsync(username);
            return Ok(user);
        }

        
        /// Gets a paginated list of users with optional filtering.
        
        [HttpGet]
        [Authorize(Policy = "VIEW_USERS")]
        [ProducesResponseType(typeof(PaginatedResult<UserReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<UserReadOnlyDTO>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] UserFiltersDTO? filters = null)
        {
            var result = await _applicationService.UserService
                .GetPaginatedUsersFilteredAsync(pageNumber, pageSize, filters ?? new UserFiltersDTO());

            return Ok(result);
        }

        private void EnsureCanViewUser(int targetUserId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isOwnProfile = currentUserId == targetUserId;

            EnsureCanViewUserCore(isOwnProfile);
        }

        private void EnsureCanViewUser(string username)
        {
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var isOwnProfile = string.Equals(currentUsername, username, StringComparison.OrdinalIgnoreCase);

            EnsureCanViewUserCore(isOwnProfile);
        }

        private void EnsureCanViewUserCore(bool isOwnProfile)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Self-view: Veterinarians και Owners μπορούν να δουν τον εαυτό τους
            if (isOwnProfile && (currentUserRole == "VETERINARIAN" || currentUserRole == "OWNER"))
            {
                return;
            }

            // Admin και Receptionist έχουν capability να δουν άλλους
            var canViewOthers = User.HasClaim("capability", "VIEW_USERS");

            if (canViewOthers)
            {
                return;
            }

            throw new EntityForbiddenException("User",
                "You do not have permission to view this user.");
        }
    }
}
