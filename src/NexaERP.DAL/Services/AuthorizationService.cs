using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NexaERP.DAL.Identity;

namespace NexaERP.DAL.Services;

public sealed class AuthorizationService(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    // Returns all permissions assigned to a user.
    public async Task<HashSet<string>> GetPermissionsForUserAsync(
        string identityId)
    {
        // Find the Identity user.
        IdentityUser? user =
            await userManager.FindByIdAsync(identityId);

        if (user is null)
        {
            return [];
        }

        // Get the user's roles.
        IList<string> roles =
            await userManager.GetRolesAsync(user);

        HashSet<string> permissions = [];

        // Collect permissions from each assigned role.
        foreach (string roleName in roles)
        {
            IdentityRole? role =
                await roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                continue;
            }

            // Get role claims.
            IList<Claim> claims =
                await roleManager.GetClaimsAsync(role);

            // Extract permission claims.
            permissions.UnionWith(
                claims
                    .Where(c => c.Type == JwtCustomClaimNames.Permission)
                    .Select(c => c.Value));
        }

        return permissions;
    }
}
