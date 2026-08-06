using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NexaERP.DAL.Caching;
using NexaERP.DAL.Identity;

namespace NexaERP.DAL.Services;

public sealed class AuthorizationService(
    CacheService cacheService,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    // Returns all permissions assigned to a user.
    public async Task<HashSet<string>> GetPermissionsForUserAsync(
        string identityId)
    {
        // Cache key for the user's permissions.
        string cacheKey = $"auth:permissions:{identityId}";

        // Try to load permissions from the cache.
        var cachedPermissions =
            await cacheService.GetAsync<HashSet<string>>(cacheKey);

        // Return cached permissions if available.
        if (cachedPermissions is not null)
        {
            return cachedPermissions;
        }

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

        // Cache the resolved permissions.
        await cacheService.SetAsync(
            cacheKey,
            permissions,
            TimeSpan.FromMinutes(15));

        return permissions;
    }
}
