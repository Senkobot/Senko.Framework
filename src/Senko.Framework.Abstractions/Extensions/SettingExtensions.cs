using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Framework.Repositories;

namespace Senko.Framework
{
    public static class SettingExtensions
    {
        public static async Task<int> GetInt32Async(this ISettingRepository repository, ulong guildId, string key, int defaultValue = 0)
        {
            return int.TryParse(await repository.GetAsync(guildId, key), out var value) ? value : defaultValue;
        }

        public static Task SetAsync(this ISettingRepository repository, ulong guildId, string key, int value)
        {
            return repository.SetAsync(guildId, key, value.ToString());
        }

        public static async Task<bool> GetBoolAsync(this ISettingRepository repository, ulong guildId, string key, bool defaultValue = false)
        {
            return bool.TryParse(await repository.GetAsync(guildId, key), out var value) ? value : defaultValue;
        }

        public static Task SetAsync(this ISettingRepository repository, ulong guildId, string key, bool value)
        {
            return repository.SetAsync(guildId, key, value.ToString());
        }

        public static async Task<ulong> GetUInt64Async(this ISettingRepository repository, ulong guildId, string key, ulong defaultValue = 0)
        {
            return ulong.TryParse(await repository.GetAsync(guildId, key), out var value) ? value : defaultValue;
        }

        public static Task SetAsync(this ISettingRepository repository, ulong guildId, string key, ulong value)
        {
            return repository.SetAsync(guildId, key, value.ToString());
        }
    }
}
