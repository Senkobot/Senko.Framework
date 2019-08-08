using System.Collections.Generic;
using Senko.Discord.Packets;
using Senko.Framework;

namespace Senko.Commands
{
    [Configuration("Permissions")]
    public class PermissionOptions
    {
        /// <summary>
        ///     The user ID's that can access the <see cref="PermissionGroup.Developer"/> commands.
        /// </summary>
        public List<ulong> Developers { get; set; } = new List<ulong>();

        /// <summary>
        ///     The guild permission that the user should have for the permission group <see cref="PermissionGroup.Moderator"/>.
        /// </summary>
        public GuildPermission ModeratorPermission { get; set; } = GuildPermission.KickMembers;

        /// <summary>
        ///     The guild permission that the user should have for the permission group <see cref="PermissionGroup.Moderator"/>.
        /// </summary>
        public GuildPermission AdministratorPermission { get; set; } = GuildPermission.Administrator;
    }
}
