using Senko.Discord;

namespace Senko.Framework.Events
{
    public class ChannelUpdateEvent : IGuildEvent
    {
        public ChannelUpdateEvent(IDiscordChannel channel)
        {
            Channel = channel;
        }

        public IDiscordChannel Channel { get; }

        ulong? IGuildEvent.GuildId => (Channel as IDiscordGuildChannel)?.GuildId;
    }
}