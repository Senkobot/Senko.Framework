using System.Runtime.CompilerServices;

namespace Senko.Common
{
    /// <summary>
    ///     All the cache keys used in Senko.
    /// </summary>
    public static class CacheKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetUserPermissionCacheKey(ulong? guildId, ulong userId) 
            => guildId.HasValue ? $"senko:guild:{guildId}:user_permissions:{userId}" : $"senko:direct_messages:user_permissions:{userId}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetChannelPermissionCacheKey(ulong? guildId, ulong channelId)
            => guildId.HasValue ? $"senko:guild:{guildId}:channel_permissions:{channelId}" : $"senko:direct_messages:channel_permissions:{channelId}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetUserExperienceCacheKey(ulong guildId, ulong userId)
            => $"senko:guild:{guildId}:user_experience:{userId}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetEnabledModulesCacheKey(ulong guildId)
            => $"senko:guild:{guildId}:enabled_modules";
    }
}
