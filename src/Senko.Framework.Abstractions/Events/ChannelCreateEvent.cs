using Senko.Discord;

namespace Senko.Framework.Events
{
    public class ChannelCreateEvent : IGuildEvent
    {
        public ChannelCreateEvent(IDiscordChannel channel)
        {
            Channel = channel;
        }

        public IDiscordChannel Channel { get; }

        ulong? IGuildEvent.GuildId => (Channel as IDiscordGuildChannel)?.GuildId;
    }
}
