using Senko.Discord.Packets;

namespace Senko.Framework.Events
{
    public class MessageEmojiDeleteEvent : IGuildEvent
    {
        public MessageEmojiDeleteEvent(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            GuildId = guildId;
            ChannelId = channelId;
            MessageId = messageId;
            Emoji = emoji;
        }

        public ulong ChannelId { get; }

        public ulong MessageId { get; }

        public DiscordEmoji Emoji { get; }

        public ulong? GuildId { get; }
    }
}
