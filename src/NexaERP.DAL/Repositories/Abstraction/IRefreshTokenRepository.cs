using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);

    Task<RefreshToken?> GetByTokenAsync(string token);

    void Update(RefreshToken refreshToken);

    void Delete(RefreshToken refreshToken);
}
