using Senko.Events;

namespace Senko.Discord.Events
{
    public class MessageReceivedEvent : IGuildEvent, IEventCancelable
    {
        private readonly IDiscordMessage _message;

        public MessageReceivedEvent(IDiscordMessage message)
        {
            _message = message;
        }

        public ulong Id => _message.Id;

        public string Content => _message.Content;

        public ulong? GuildId => _message.GuildId;

        public ulong ChannelId => _message.ChannelId;

        public IDiscordUser Author => _message.Author;

        public bool IsCancelled { get; set; }
    }
}
