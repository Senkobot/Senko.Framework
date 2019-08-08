using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Discord
{
    public class DiscordGuildUser : IDiscordGuildUser
    {
        private readonly List<ulong> _roleIds = new List<ulong>();
        public readonly IDiscordUser User;
        
        public DiscordGuild Guild;

        public DiscordGuildUser(IDiscordUser user, DiscordGuild guild = null)
        {
            User = user;
            Guild = guild;
        }

        public string Nickname { get; set; }

        public IReadOnlyCollection<ulong> RoleIds => _roleIds;

        public ulong GuildId => Guild.Id;

        public DateTimeOffset JoinedAt { get; set; }

        public DateTimeOffset? PremiumSince { get; set; }

        public GuildPermission UserPermissions { get; set; } 

        public Task AddRoleAsync(IDiscordRole role)
        {
            _roleIds.Add(role.Id);

            if (Guild == null)
            {
                return Task.CompletedTask;
            }

            return Guild.Client.EventHandler.OnGuildMemberRolesUpdate(this);
        }

        public Task<IDiscordGuild> GetGuildAsync()
        {
            return Task.FromResult<IDiscordGuild>(Guild);
        }

        public Task<int> GetHierarchyAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPermissionsAsync(GuildPermission permissions)
        {
            throw new NotImplementedException();
        }

        public Task KickAsync(string reason = "")
        {
            throw new NotImplementedException();
        }

        public Task RemoveRoleAsync(IDiscordRole role)
        {
            _roleIds.RemoveAll(id => role.Id == id);
            return Task.CompletedTask;
        }

        protected bool Equals(DiscordGuildUser other)
        {
            return base.Equals(other) && Equals(User, other.User);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is IDiscordUser user) return user.Equals(User);
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiscordGuildUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (User != null ? User.GetHashCode() : 0);
            }
        }

        #region IDiscordUser

        public ulong Id => User.Id;

        public string AvatarId => User.AvatarId;

        public string Mention => User.Mention;

        public string Username => User.Username;

        public string Discriminator => User.Discriminator;

        public DateTimeOffset CreatedAt => User.CreatedAt;

        public bool IsBot => User.IsBot;

        public Task<IDiscordPresence> GetPresenceAsync()
        {
            return User.GetPresenceAsync();
        }

        public Task<IDiscordTextChannel> GetDMChannelAsync()
        {
            return User.GetDMChannelAsync();
        }

        public string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256)
        {
            return User.GetAvatarUrl(type, size);
        }

        public DiscordUserPacket Packet { get; }

        #endregion
    }
}
