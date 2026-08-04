using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class RefreshTokenRepository(
    ApplicationIdentityDbContext context) : IRefreshTokenRepository
{
    // Adds a new refresh token.
    public async Task AddAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
    }

    // Returns a refresh token by its token value.
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    // Updates an existing refresh token.
    public void Update(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
    }

    // Deletes a refresh token.
    public void Delete(RefreshToken refreshToken)
    {
        context.RefreshTokens.Remove(refreshToken);
    }
}
