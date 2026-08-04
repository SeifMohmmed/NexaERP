using System.Security.Claims;

namespace NexaERP.DAL.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to simplify access to common claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Retrieves the IdentityId (NameIdentifier claim) from the current user principal.
    /// </summary>
    public static string? GetIdentityId(this ClaimsPrincipal? principal)
    {
        // Find the NameIdentifier claim (usually the unique user ID from Identity provider)
        string? identityId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return identityId;
    }
}
