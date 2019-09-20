using System.Threading.Tasks;
using Senko.Framework.Managers;
using Senko.Framework.Repositories;

namespace Senko.Framework
{
    public static class SettingExtensions
    {
        public static async Task<IGuildOptions<T>> GetOptionsAsync<T>(this IGuildOptionsManager manager, ulong guildId)
            where T : class, new()
        {
            return new GuildOptions<T>(manager, guildId, await manager.GetAsync<T>(guildId));
        }

        public static async Task<int> GetInt32Async(this IGuildOptionRepository repository, ulong guildId, string key, int defaultValue = 0)
        {
            return int.TryParse(await repository.GetAsync(guildId, key), out var value) ? value : defaultValue;
        }

        public static Task SetAsync(this IGuildOptionRepository repository, ulong guildId, string key, int value)
        {
            return repository.SetAsync(guildId, key, value.ToString());
        }

        public static async Task<bool> GetBoolAsync(this IGuildOptionRepository repository, ulong guildId, string key, bool defaultValue = false)
        {
            return bool.TryParse(await repository.GetAsync(guildId, key), out var value) ? value : defaultValue;
        }

        public static Task SetAsync(this IGuildOptionRepository repository, ulong guildId, string key, bool value)
        {
            return repository.SetAsync(guildId, key, value.ToString());
        }

        public static async Task<ulong> GetUInt64Async(this IGuildOptionRepository repository, ulong guildId, string key, ulong defaultValue = 0)
        {
            return ulong.TryParse(await repository.GetAsync(guildId, key), out var value) ? value : defaultValue;
        }

        public static Task SetAsync(this IGuildOptionRepository repository, ulong guildId, string key, ulong value)
        {
            return repository.SetAsync(guildId, key, value.ToString());
        }
    }
}
