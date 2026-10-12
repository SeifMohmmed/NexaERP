using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Database;

namespace NexaERP.BLL.Services.Implementation;

public sealed class IdentityRecipientService(
    ApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager)
    : IIdentityRecipientService
{
    public async Task<IReadOnlyList<Guid>> GetUserIdsByRoleAsync(
        string roleName,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        IList<IdentityUser> identityUsers =
            await userManager.GetUsersInRoleAsync(roleName);

        string[] identityIds = identityUsers
            .Select(user => user.Id)
            .ToArray();

        return await applicationDbContext.Users
            .AsNoTracking()
            .Where(user => identityIds.Contains(user.IdentityId))
            .Select(user => user.Id)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<bool> UserExistsAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        return await applicationDbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId, ct);
    }
}
