using System.Linq;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;
using Xunit;

namespace Senko.TestFramework.Discord
{
    public class DiscordGuildTextChannel : DiscordTextChannel, IDiscordGuildChannel
    {
        public override ulong? GuildId => Guild?.Id;

        ulong IDiscordGuildChannel.GuildId => Guild.Id;

        public IDiscordGuild Guild { get; set; }

        public ChannelType Type => ChannelType.GUILDTEXT;

        public ValueTask<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
        {
            return Guild.GetPermissionsAsync(user);
        }

        public ValueTask<IDiscordGuildUser> GetUserAsync(ulong id)
        {
            return Guild.GetMemberAsync(id);
        }

        public ValueTask<IDiscordGuild> GetGuildAsync()
        {
            return new ValueTask<IDiscordGuild>(Guild);
        }

        public void AssertLastMessage(string content)
        {
            var message = Messages.LastOrDefault();

            Assert.Equal(content, message?.Content ?? message?.Embed?.Description);
        }
    }
}
