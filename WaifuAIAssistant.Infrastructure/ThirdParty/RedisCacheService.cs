using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    public class RedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _cache.GetStringAsync(key);
            return value is null ? default : JsonSerializer.Deserialize<T>(value, _options);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var serialized = JsonSerializer.Serialize(value, _options);
            var opts = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };
            await _cache.SetStringAsync(key, serialized, opts);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }

}
