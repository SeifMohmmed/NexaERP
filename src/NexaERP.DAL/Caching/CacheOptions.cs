using Microsoft.Extensions.Caching.Distributed;

namespace NexaERP.DAL.Caching;

public static class CacheOptions
{
    // Default cache expiration.
    private readonly static DistributedCacheEntryOptions DefaultExpiration = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };

    // Creates cache options with an optional custom expiration.
    public static DistributedCacheEntryOptions Create(TimeSpan? expiration = null) =>
        expiration is not null
            ? new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            }
            : DefaultExpiration;
}
