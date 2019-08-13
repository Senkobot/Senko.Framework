using System.Collections.Generic;
using System.Threading.Tasks;

namespace Senko.Commands.Managers
{
    public interface IPermissionManager
    {
        /// <summary>
        ///     All the registered permissions.
        /// </summary>
        IReadOnlyList<string> Permissions { get; }

        /// <summary>
        ///     Get the permission groups where the user is member of.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="guildId">The guild ID.</param>
        /// <returns>The permission groups of the user.</returns>
        Task<IReadOnlyList<PermissionGroup>> GetPermissionGroups(ulong userId, ulong? guildId = null);

        /// <summary>
        ///     Get the permissions where the user has access to.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="guildId">The guild ID.</param>
        /// <returns>All the permissions where the user has access to.</returns>
        Task<IReadOnlyCollection<string>> GetAllowedUserPermissionAsync(ulong userId, ulong? guildId = null);

        /// <summary>
        ///     Get the permissions that the users can execute in the channel.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="guildId">The guild ID.</param>
        /// <returns>All the permissions where the channel has access to.</returns>
        Task<IReadOnlyCollection<string>> GetAllowedChannelPermissionAsync(ulong channelId, ulong? guildId = null);

        /// <summary>
        ///     Check if the user has access to the <see cref="permission"/>.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="permission">The permission to check.</param>
        /// <param name="guildId">The guild ID.</param>
        /// <returns>True if the user has access to the <see cref="permission"/>.</returns>
        Task<bool> HasUserPermissionAsync(ulong userId, string permission, ulong? guildId = null);

        /// <summary>
        ///     Check if the channel has access to the <see cref="permission"/>.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="permission">The permission to check.</param>
        /// <param name="guildId">The guild ID.</param>
        /// <returns>True if the user has access to the <see cref="permission"/>.</returns>
        Task<bool> HasChannelPermissionAsync(ulong channelId, string permission, ulong? guildId = null);

        Task<bool> SetUserPermissionAsync(ulong guildId, ulong userId, string permissionName, bool? granted);

        Task<bool> SetRolePermissionAsync(ulong guildId, ulong roleId, string permissionName, bool? granted);

        Task<bool> SetChannelPermissionAsync(ulong guildId, ulong channelId, string permissionName, bool? granted);
    }
}
