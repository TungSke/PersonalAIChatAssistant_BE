using System;
using System.Collections.Generic;
using System.Text;

namespace WaifuAIAssistant.Domain.ThirdPartyInterface
{
    public interface IRedisCacheService
    {
        Task<T?> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value, TimeSpan ttl);

        Task RemoveAsync(string key);
    }
}
