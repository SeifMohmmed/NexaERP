using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Roles;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Caching;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.BLL.Services.Implementation;

public sealed class RoleService(
    RoleManager<IdentityRole> roleManager,
    UserManager<IdentityUser> userManager,
    CacheService cacheService,
    IUserRepository userRepository)
    : IRoleService
{
    // Returns all available roles.
    public async Task<IReadOnlyCollection<RoleDto>> GetRolesAsync()
    {
        const string cacheKey = "roles:all";

        IReadOnlyCollection<RoleDto>? cachedRoles =
            await cacheService.GetAsync<IReadOnlyCollection<RoleDto>>(cacheKey);

        if (cachedRoles is not null)
        {
            return cachedRoles;
        }

        IReadOnlyCollection<RoleDto> roles =
            await roleManager.Roles
                .AsNoTracking()
                .Select(role => new RoleDto
                {
                    Name = role.Name!
                })
                .ToListAsync();

        await cacheService.SetAsync(
            cacheKey,
            roles,
            TimeSpan.FromMinutes(30));

        return roles;
    }

    // Assigns a role to a user.
    public async Task<Result> AssignRoleAsync(
        AssignRoleDto dto)
    {
        // Find the application user.
        User? user = await GetUserAsync(dto.UserId);

        if (user is null)
        {
            return CreateFailedResult(
                "UserNotFound",
                "User was not found.");
        }

        // Find the Identity user.
        IdentityUser? identityUser =
            await GetIdentityUserAsync(user);

        if (identityUser is null)
        {
            return CreateFailedResult(
                "IdentityUserNotFound",
                "Identity user was not found.");
        }

        // Validate the requested role.
        Result? validation =
            await ValidateRoleAsync(dto.Role);

        if (validation is not null)
        {
            return validation;
        }

        // Assign the role.
        IdentityResult result =
            await userManager.AddToRoleAsync(
                identityUser,
                dto.Role);

        // Return Identity errors if the operation fails.
        if (!result.Succeeded)
        {
            return CreateFailedResult(result);
        }

        // Invalidate cached permissions.
        await cacheService.RemoveAsync(
            $"auth:permissions:{identityUser.Id}");

        return new Result
        {
            Succeeded = true
        };
    }

    // Returns all roles assigned to a user.
    public async Task<IReadOnlyCollection<RoleDto>> GetUserRolesAsync(
        Guid userId)
    {
        // Find the application user.
        User? user =
            await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return [];
        }

        // Find the Identity user.
        IdentityUser? identityUser =
            await userManager.FindByIdAsync(user.IdentityId);

        if (identityUser is null)
        {
            return [];
        }

        // Get assigned roles.
        IList<string> roles =
            await userManager.GetRolesAsync(identityUser);

        return roles
            .Select(role => new RoleDto
            {
                Name = role
            })
            .ToList();
    }

    // Removes a role from a user.
    public async Task<Result> RemoveRoleAsync(RemoveRoleDto dto)
    {
        // Find the application user.
        User? user = await GetUserAsync(dto.UserId);

        if (user is null)
        {
            return CreateFailedResult(
                "UserNotFound",
                "User was not found.");
        }

        // Find the Identity user.
        IdentityUser? identityUser =
            await GetIdentityUserAsync(user);

        if (identityUser is null)
        {
            return CreateFailedResult(
                "IdentityUserNotFound",
                "Identity user was not found.");
        }

        // Validate the requested role.
        Result? validation =
            await ValidateRoleAsync(dto.Role);

        if (validation is not null)
        {
            return validation;
        }

        // Remove the role.
        IdentityResult result =
            await userManager.RemoveFromRoleAsync(
                identityUser,
                dto.Role);

        // Return Identity errors if the operation fails.
        if (!result.Succeeded)
        {
            return CreateFailedResult(result);

        }

        // Invalidate cached permissions.
        await cacheService.RemoveAsync(
            $"auth:permissions:{identityUser.Id}");

        return new Result
        {
            Succeeded = true
        };
    }

    // Replaces the user's assigned roles.
    public async Task<Result> UpdateUserRolesAsync(
        Guid userId,
        UpdateUserRolesDto dto)
    {
        // Find the application user.
        User? user = await GetUserAsync(userId);

        if (user is null)
        {
            return CreateFailedResult(
                "UserNotFound",
                "User was not found.");
        }

        // Find the Identity user.
        IdentityUser? identityUser =
            await GetIdentityUserAsync(user);

        if (identityUser is null)
        {
            return CreateFailedResult(
                "IdentityUserNotFound",
                "Identity user was not found.");
        }

        // Validate all requested roles.
        foreach (string role in dto.Roles)
        {
            Result? validation =
                await ValidateRoleAsync(role);

            if (validation is not null)
            {
                return validation;
            }
        }

        // Get the user's current roles.
        IList<string> currentRoles =
            await userManager.GetRolesAsync(identityUser);

        // Determine roles to add and remove.
        IEnumerable<string> rolesToAdd =
            dto.Roles.Except(currentRoles);

        IEnumerable<string> rolesToRemove =
            currentRoles.Except(dto.Roles);

        // Remove unassigned roles.
        IdentityResult removeResult =
            await userManager.RemoveFromRolesAsync(
                identityUser,
                rolesToRemove);

        if (!removeResult.Succeeded)
        {
            return CreateFailedResult(removeResult);
        }

        // Add newly assigned roles.
        IdentityResult addResult =
            await userManager.AddToRolesAsync(
                identityUser,
                rolesToAdd);

        if (!addResult.Succeeded)
        {
            return CreateFailedResult(addResult);
        }

        // Invalidate cached permissions.
        await cacheService.RemoveAsync(
            $"auth:permissions:{identityUser.Id}");

        return new Result
        {
            Succeeded = true
        };
    }

    // Returns the application user.
    private async Task<User?> GetUserAsync(Guid userId)
    {
        return await userRepository.GetByIdAsync(userId);
    }

    // Returns the corresponding Identity user.
    private async Task<IdentityUser?> GetIdentityUserAsync(User user)
    {
        return await userManager.FindByIdAsync(user.IdentityId);
    }

    // Creates a failed result with a custom error.
    private static Result CreateFailedResult(
        string code,
        string message)
    {
        return new Result
        {
            Succeeded = false,
            Errors = new Dictionary<string, string>
            {
                [code] = message
            }
        };
    }

    // Creates a failed result from Identity errors.
    private static Result CreateFailedResult(
        IdentityResult result)
    {
        return new Result
        {
            Succeeded = false,
            Errors = result.Errors.ToDictionary(
                e => e.Code,
                e => e.Description)
        };
    }

    // Validates that the specified role exists.
    private async Task<Result?> ValidateRoleAsync(string role)
    {
        if (await roleManager.RoleExistsAsync(role))
        {
            return null;
        }

        return CreateFailedResult(
            "RoleNotFound",
            $"Role '{role}' does not exist.");
    }
}
