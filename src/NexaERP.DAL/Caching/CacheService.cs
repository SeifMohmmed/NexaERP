using System.Buffers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace NexaERP.DAL.Caching;

public class CacheService(IDistributedCache cache)
{
    // Returns a cached value.
    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await cache.GetAsync(
            key,
            cancellationToken);

        return bytes is null
            ? default
            : Deserialize<T>(bytes);
    }

    // Stores a value in the cache.
    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        return cache.SetAsync(
            key,
            bytes,
            CacheOptions.Create(expiration),
            cancellationToken);
    }

    // Removes a cached value.
    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(key, cancellationToken);
    }

    // Deserializes cached data.
    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }

    // Serializes data for caching.
    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();

        using var writer = new Utf8JsonWriter(buffer);

        JsonSerializer.Serialize(writer, value);

        return buffer.WrittenSpan.ToArray();
    }
}
