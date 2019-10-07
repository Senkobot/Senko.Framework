using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.DependencyInjection;
using Senko.Framework.Attributes;
using Senko.Framework.Repositories;

namespace Senko.Framework.Managers
{
    public class GuildOptionsManager : IGuildOptionsManager
    {
        private static readonly ConcurrentDictionary<Type, string> CachedKeys = new ConcurrentDictionary<Type, string>();

        private readonly IHybridCacheClient _hybridCacheClient;
        private readonly IServiceProvider _provider;

        public GuildOptionsManager(IHybridCacheClient hybridCacheClient, IServiceProvider provider)
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

        private static string GetKey(Type type)
        {
            return CachedKeys.GetOrAdd(type, t =>
            {
                var attribute = t.GetCustomAttribute<OptionsKeyAttribute>();

                return attribute?.Key ?? (t.Assembly.GetName().Name + '.' + t.Name);
            });
        }

        public async ValueTask<T> GetAsync<T>(ulong guildId) where T : new()
        {
            var key = GetKey(typeof(T));
            var cacheKey = GetCacheKey(guildId, key);
            var cacheItem = await _hybridCacheClient.GetAsync<T>(cacheKey);

            if (cacheItem.HasValue && !cacheItem.IsNull)
            {
                return cacheItem.Value;
            }

            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IGuildOptionRepository>();
            var json = await repo.GetAsync(guildId, key);
            var value = string.IsNullOrEmpty(json) ? new T() : JsonSerializer.Deserialize<T>(json);

            await _hybridCacheClient.SetAsync(cacheKey, value, CacheTime);

            return value;
        }

        public async ValueTask SetAsync<T>(ulong guildId, T value)
        {
            var key = GetKey(typeof(T));
            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IGuildOptionRepository>();
            var json = JsonSerializer.Serialize(value);

            await repo.SetAsync(guildId, key, json);
            await _hybridCacheClient.SetAsync(GetCacheKey(guildId, key), value, CacheTime);
        }
    }
}
