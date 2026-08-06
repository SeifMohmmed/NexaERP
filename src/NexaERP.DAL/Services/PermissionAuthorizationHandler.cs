using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Extensions;

namespace NexaERP.DAL.Services;

internal sealed class PermissionAuthorizationHandler(
    IServiceProvider serviceProvider)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Ensure the user is authenticated.
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            return;
        }

        // Create a scoped service provider.
        await using AsyncServiceScope scope =
            serviceProvider.CreateAsyncScope();

        // Resolve the authorization service.
        AuthorizationService authorizationService =
            scope.ServiceProvider
                .GetRequiredService<AuthorizationService>();

        // Get the current user's Identity ID.
        string? identityId = context.User.GetIdentityId();

        if (identityId is null)
        {
            return;
        }

        // Retrieve the user's permissions.
        HashSet<string> permissions =
            await authorizationService.GetPermissionsForUserAsync(
                identityId);

        // Succeed if the required permission is granted.
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
