using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Discord
{
    public class DiscordGuildTextChannel : DiscordTextChannel, IDiscordGuildChannel
    {
        public override ulong? GuildId => Guild?.Id;

        ulong IDiscordGuildChannel.GuildId => Guild.Id;

        public IDiscordGuild Guild { get; set; }

        public ChannelType Type => ChannelType.GUILDTEXT;

        public Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
        {
            return Guild.GetPermissionsAsync(user);
        }

        public Task<IDiscordGuildUser> GetUserAsync(ulong id)
        {
            return Guild.GetMemberAsync(id);
        }

        public Task<IDiscordGuild> GetGuildAsync()
        {
            return Task.FromResult(Guild);
        }
    }
}
