using Microsoft.AspNetCore.Identity;

namespace NexaERP.DAL.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; set; }

    public required string UserId { get; set; }

    public required string TokenHash { get; set; }

    public Guid FamilyId { get; set; }

    public required DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public IdentityUser User { get; set; } = default!;
}
