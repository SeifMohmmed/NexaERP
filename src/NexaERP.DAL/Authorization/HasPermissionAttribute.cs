using Microsoft.AspNetCore.Authorization;

namespace NexaERP.DAL.Authorization;

#pragma warning disable

// Applies a permission-based authorization policy.
public sealed class HasPermissionAttribute(string permission)
    : AuthorizeAttribute(permission);
