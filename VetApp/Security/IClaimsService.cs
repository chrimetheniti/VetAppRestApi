namespace VetApp.Security
{
    public interface IClaimsService
    {
        int? GetCurrentUserId();
        string? GetCurrentUsername();
        string? GetCurrentUserRole();
        int? GetCurrentVeterinarianId();
        int? GetCurrentOwnerId();
        bool HasCapability(string capability);
    }
}
