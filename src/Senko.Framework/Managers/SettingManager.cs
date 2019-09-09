using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.DependencyInjection;
using Senko.Framework.Repositories;
using SpanJson;

namespace Senko.Framework.Managers
{
    public class SettingManager : ISettingManager
    {
        private readonly IHybridCacheClient _hybridCacheClient;
        private readonly IServiceProvider _provider;

        public SettingManager(IHybridCacheClient hybridCacheClient, IServiceProvider provider)
        {
            _hybridCacheClient = hybridCacheClient;
            _provider = provider;
        }

        private TimeSpan CacheTime { get; } = TimeSpan.FromMinutes(5);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetCacheKey(ulong guildId, string key)
        {
            return $"Senko:Settings:{guildId}:{key}";
        }

        public async Task<T> GetAsync<T>(ulong guildId, string key)
        {
            var cacheKey = GetCacheKey(guildId, key);
            var cacheItem = await _hybridCacheClient.GetAsync<T>(cacheKey);

            if (cacheItem.HasValue)
            {
                return cacheItem.Value;
            }

            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
            var json = await repo.GetAsync(guildId, key);
            var value = JsonSerializer.Generic.Utf16.Deserialize<T>(json);

            await _hybridCacheClient.SetAsync(cacheKey, value, CacheTime);

            return value;
        }

        public async Task SetAsync<T>(ulong guildId, string key, T value)
        {
            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
            var json = JsonSerializer.Generic.Utf16.Serialize(value);

            await repo.SetAsync(guildId, key, json);
            await _hybridCacheClient.SetAsync(GetCacheKey(guildId, key), value, CacheTime);
        }
    }
}
