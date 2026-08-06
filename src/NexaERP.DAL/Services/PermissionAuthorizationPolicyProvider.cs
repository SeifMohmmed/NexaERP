using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NexaERP.DAL.Authorization;

namespace NexaERP.DAL.Services;

internal sealed class PermissionAuthorizationPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _authorizationOptions;

    public PermissionAuthorizationPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
        _authorizationOptions = options.Value;
    }

    // Returns an existing policy or creates a permission-based policy.
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        // Check whether the policy already exists.
        AuthorizationPolicy? policy =
            await base.GetPolicyAsync(policyName);

        if (policy is not null)
        {
            return policy;
        }

        // Create a new permission policy.
        AuthorizationPolicy permissionPolicy =
            new AuthorizationPolicyBuilder()
                .AddRequirements(
                    new PermissionRequirement(policyName))
                .Build();

        // Cache the generated policy.
        _authorizationOptions.AddPolicy(
            policyName,
            permissionPolicy);

        return permissionPolicy;
    }
}
