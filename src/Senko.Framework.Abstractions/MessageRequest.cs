namespace Senko.Framework
{
    public abstract class MessageRequest
    {
        public abstract MessageContext Context { get; }

        public abstract string Message { get; set; }

        public abstract ulong MessageId { get; set; }

        public abstract ulong ChannelId { get; set; }

        public abstract ulong? GuildId { get; set; }
    }
}