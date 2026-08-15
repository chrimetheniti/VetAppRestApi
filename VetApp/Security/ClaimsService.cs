using System.Security.Claims;

namespace VetApp.Security
{
    public class ClaimsService : IClaimsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public int? GetCurrentUserId()
        {
            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }

        public string? GetCurrentUsername()
        {
            return User?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public string? GetCurrentUserRole()
        {
            return User?.FindFirst(ClaimTypes.Role)?.Value;
        }

        public int? GetCurrentVeterinarianId()
        {
            var value = User?.FindFirst("veterinarianId")?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }

        public int? GetCurrentOwnerId()
        {
            var value = User?.FindFirst("ownerId")?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }

        public bool HasCapability(string capability)
        {
            return User?.HasClaim("capability", capability) ?? false;
        }
    }
}
