namespace NexaERP.BLL.Services.Abstraction;

public interface IIdentityRecipientService
{
    Task<IReadOnlyList<Guid>> GetUserIdsByRoleAsync(
        string roleName,
        CancellationToken ct = default);

    Task<bool> UserExistsAsync(
        Guid userId,
        CancellationToken ct = default);
}
