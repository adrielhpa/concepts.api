using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Concepts.Repository.Helpers
{
    public static class RedisCacheRepository
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            DistributedCacheEntryOptions opt = new();

            opt.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(1);
            opt.SlidingExpiration = unusedExpireTime;

            var json = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, json, opt);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var json = await cache.GetStringAsync(recordId);

            if (json is null) return default(T);

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
