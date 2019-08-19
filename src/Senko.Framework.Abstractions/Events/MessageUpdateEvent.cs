using Senko.Discord;
using Senko.Events;

namespace Senko.Framework.Events
{
    public class MessageUpdateEvent : IGuildEvent, IEventCancelable
    {
        private readonly IDiscordMessage _message;

        public MessageUpdateEvent(IDiscordMessage message)
        {
            _message = message;
        }

        public string Content => _message.Content;

        public ulong? GuildId => _message.GuildId;

        public ulong ChannelId => _message.ChannelId;

        public IDiscordUser Author => _message.Author;

        public bool IsCancelled { get; set; }
    }
}
