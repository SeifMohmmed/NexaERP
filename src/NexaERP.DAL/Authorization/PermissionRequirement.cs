using Microsoft.AspNetCore.Authorization;

namespace NexaERP.DAL.Authorization;

internal sealed class PermissionRequirement
    : IAuthorizationRequirement
{
    // Creates a permission requirement.
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }

    // Required permission.
    public string Permission { get; }
}
