namespace NexaERP.DAL.Identity;

// Defines custom claim names used inside JWT tokens
public static class JwtCustomClaimNames
{
    // User role claim.
    public const string Role = "role";

    // User permission claim.
    public const string Permission = "permission";
}
