namespace Senko.Framework.Events
{
    public class MessageDeleteEvent : IGuildEvent
    {
        public MessageDeleteEvent(ulong channelId, ulong messageId, ulong? guildId)
        {
            ChannelId = channelId;
            MessageId = messageId;
            GuildId = guildId;
        }

        public ulong ChannelId { get; }

        public ulong MessageId { get; }

        public ulong? GuildId { get; }

        ulong? IGuildEvent.GuildId => GuildId;
    }
}