using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Commands.Managers;

namespace Senko.Commands
{
    public static class PermissionManagerExtensions
    {
        #region User
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> GrantUserPermissionAsync(this IPermissionManager manager, ulong guildId, ulong userId, string permissionName)
            => manager.SetUserPermissionAsync(guildId, userId, permissionName, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> ResetUserPermissionAsync(this IPermissionManager manager, ulong guildId, ulong userId, string permissionName)
            => manager.SetUserPermissionAsync(guildId, userId, permissionName, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> RevokeUserPermissionAsync(this IPermissionManager manager, ulong guildId, ulong userId, string permissionName)
            => manager.SetUserPermissionAsync(guildId, userId, permissionName, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> GrantUserPermissionAsync(this IPermissionManager manager, IDiscordGuildUser user, string permissionName)
            => manager.SetUserPermissionAsync(user.GuildId, user.Id, permissionName, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> ResetUserPermissionAsync(this IPermissionManager manager, IDiscordGuildUser user, string permissionName)
            => manager.SetUserPermissionAsync(user.GuildId, user.Id, permissionName, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> RevokeUserPermissionAsync(this IPermissionManager manager, IDiscordGuildUser user, string permissionName)
            => manager.SetUserPermissionAsync(user.GuildId, user.Id, permissionName, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> HasUserPermissionAsync(this IPermissionManager manager, IDiscordGuildUser user, string permissionName)
            => manager.HasUserPermissionAsync(user.Id, permissionName, user.GuildId);
        #endregion

        #region Role
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> GrantRolePermissionAsync(this IPermissionManager manager, ulong guildId, ulong roleId, string permissionName)
            => manager.SetRolePermissionAsync(guildId, roleId, permissionName, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> RevokeRolePermissionAsync(this IPermissionManager manager, ulong guildId, ulong roleId, string permissionName)
            => manager.SetRolePermissionAsync(guildId, roleId, permissionName, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> ResetRolePermissionAsync(this IPermissionManager manager, ulong guildId, ulong roleId, string permissionName)
            => manager.SetRolePermissionAsync(guildId, roleId, permissionName, null);
        #endregion

        #region Channel
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> GrantChannelPermissionAsync(this IPermissionManager manager, ulong guildId, ulong channelId, string permissionName)
            => manager.SetChannelPermissionAsync(guildId, channelId, permissionName, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> RevokeChannelPermissionAsync(this IPermissionManager manager, ulong guildId, ulong channelId, string permissionName)
            => manager.SetChannelPermissionAsync(guildId, channelId, permissionName, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> ResetChannelPermissionAsync(this IPermissionManager manager, ulong guildId, ulong channelId, string permissionName)
            => manager.SetChannelPermissionAsync(guildId, channelId, permissionName, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> GrantChannelPermissionAsync(this IPermissionManager manager, IDiscordGuildChannel channel, string permissionName)
            => manager.SetChannelPermissionAsync(channel.GuildId, channel.Id, permissionName, true);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> RevokeChannelPermissionAsync(this IPermissionManager manager, IDiscordGuildChannel channel, string permissionName)
            => manager.SetChannelPermissionAsync(channel.GuildId, channel.Id, permissionName, false);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> ResetChannelPermissionAsync(this IPermissionManager manager, IDiscordGuildChannel channel, string permissionName)
            => manager.SetChannelPermissionAsync(channel.GuildId, channel.Id, permissionName, null);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> HasChannelPermissionAsync(this IPermissionManager manager, IDiscordGuildChannel channel, string permissionName)
            => manager.HasChannelPermissionAsync(channel.Id, permissionName, channel.GuildId);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> HasRolePermissionAsync(this IPermissionManager manager, IDiscordRole role, ulong guildId, string permissionName)
            => manager.HasRolePermissionAsync(role.Id, permissionName, guildId);
        #endregion
    }
}
