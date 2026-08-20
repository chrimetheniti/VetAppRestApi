using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetApp.DTO;
using VetApp.Exceptions;
using VetApp.Services;

namespace VetApp.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IConfiguration _configuration;

        public AuthController(IApplicationService applicationService, IConfiguration configuration)
        {
            _applicationService = applicationService;
            _configuration = configuration;
        }


        /// Registers a new veterinarian account.
        /// Only Admin and Receptionist can create vet accounts — this is staff onboarding,
        /// not public self-signup (unlike owner registration below).

        [HttpPost("register/veterinarian")]
        [Authorize(Roles = "ADMIN,RECEPTIONIST")]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserReadOnlyDTO>> RegisterVeterinarian(
            [FromBody] VeterinarianSignupDTO veterinarianSignupDTO)
        {
            var createdUser = await _applicationService.VeterinarianService
                .SignUpUserAsync(veterinarianSignupDTO);
            return CreatedAtAction(
                actionName: nameof(UsersController.GetUserById),
                controllerName: "Users",
                routeValues: new { id = createdUser.Id },
                value: createdUser);
        }


        /// Registers a new owner account.
        /// Public endpoint — anyone can sign up as an owner (customer self-service).
        /// Admin/Receptionist also use this endpoint via the admin UI.

        [HttpPost("register/owner")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserReadOnlyDTO>> RegisterOwner(
            [FromBody] OwnerSignupDTO ownerSignupDTO)
        {
            var createdUser = await _applicationService.OwnerService
                .SignUpUserAsync(ownerSignupDTO);
            return CreatedAtAction(
                actionName: nameof(UsersController.GetUserById),
                controllerName: "Users",
                routeValues: new { id = createdUser.Id },
                value: createdUser);
        }


        /// Authenticates a user and returns a JWT token.

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JwtTokenDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<JwtTokenDTO>> Login(
            [FromBody] UserLoginDTO credentials)
        {
            var user = await _applicationService.UserService
                .VerifyAndGetUserAsync(credentials)
                ?? throw new EntityNotAuthorizedException("User", "Bad Credentials");
            var token = _applicationService.UserService.CreateUserToken(user);
            return Ok(new JwtTokenDTO(token));
        }
    }
}