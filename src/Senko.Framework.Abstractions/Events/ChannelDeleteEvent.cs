using Senko.Discord;

namespace Senko.Framework.Events
{
    public class ChannelDeleteEvent : IGuildEvent
    {
        public ChannelDeleteEvent(IDiscordChannel channel)
        {
            Channel = channel;
        }

        public IDiscordChannel Channel { get; }

        ulong? IGuildEvent.GuildId => (Channel as IDiscordGuildChannel)?.GuildId;
    }
}