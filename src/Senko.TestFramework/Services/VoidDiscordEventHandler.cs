using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Services
{
    public class VoidDiscordEventHandler : IDiscordEventHandler
    {
        public Task OnGuildJoin(IDiscordGuild guild)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildUpdate(IDiscordGuild guild)
        {
            return Task.CompletedTask;
        }

        public Task OnUserUpdate(IDiscordUser user)
        {
            return Task.CompletedTask;
        }

        public Task OnChannelCreate(IDiscordChannel channel)
        {
            return Task.CompletedTask;
        }

        public Task OnChannelUpdate(IDiscordChannel channel)
        {
            return Task.CompletedTask;
        }

        public Task OnChannelDelete(IDiscordChannel channel)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildUnavailable(ulong guildId)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildLeave(ulong guildId)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberDelete(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberUpdate(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberCreate(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildRoleCreate(ulong guildId, IDiscordRole role)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildRoleUpdate(ulong guildId, IDiscordRole role)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildRoleDeleted(ulong guildId, IDiscordRole role)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageCreate(IDiscordMessage message)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageUpdate(IDiscordMessage message)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageDeleted(ulong channelId, ulong messageId)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageEmojiCreated(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageEmojiDeleted(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberRolesUpdate(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }
    }
}
