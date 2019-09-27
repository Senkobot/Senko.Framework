using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Services
{
    public class VoidDiscordEventHandler : IDiscordEventHandler
    {
        public ValueTask OnGuildJoin(IDiscordGuild guild)
        {
            return default;
        }

        public ValueTask OnGuildUpdate(IDiscordGuild guild)
        {
            return default;
        }

        public ValueTask OnUserUpdate(IDiscordUser user)
        {
            return default;
        }

        public ValueTask OnChannelCreate(IDiscordChannel channel)
        {
            return default;
        }

        public ValueTask OnChannelUpdate(IDiscordChannel channel)
        {
            return default;
        }

        public ValueTask OnChannelDelete(IDiscordChannel channel)
        {
            return default;
        }

        public ValueTask OnGuildUnavailable(ulong guildId)
        {
            return default;
        }

        public ValueTask OnGuildLeave(ulong guildId)
        {
            return default;
        }

        public ValueTask OnGuildMemberDelete(IDiscordGuildUser member)
        {
            return default;
        }

        public ValueTask OnGuildMemberUpdate(IDiscordGuildUser member)
        {
            return default;
        }

        public ValueTask OnGuildMemberCreate(IDiscordGuildUser member)
        {
            return default;
        }

        public ValueTask OnGuildRoleCreate(ulong guildId, IDiscordRole role)
        {
            return default;
        }

        public ValueTask OnGuildRoleUpdate(ulong guildId, IDiscordRole role)
        {
            return default;
        }

        public ValueTask OnGuildRoleDeleted(ulong guildId, IDiscordRole role)
        {
            return default;
        }

        public ValueTask OnMessageCreate(IDiscordMessage message)
        {
            return default;
        }

        public ValueTask OnMessageUpdate(IDiscordMessage message)
        {
            return default;
        }

        public ValueTask OnMessageDeleted(ulong channelId, ulong messageId)
        {
            return default;
        }

        public ValueTask OnMessageEmojiCreated(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return default;
        }

        public ValueTask OnMessageEmojiDeleted(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return default;
        }

        public ValueTask OnGuildMemberRolesUpdate(IDiscordGuildUser member)
        {
            return default;
        }
    }
}
