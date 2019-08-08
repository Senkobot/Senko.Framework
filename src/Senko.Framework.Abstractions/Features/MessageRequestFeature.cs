namespace Senko.Framework.Features
{
    public struct MessageRequestFeature : IMessageRequestFeature
    {
        public ulong MessageId { get; set; }

        public string Message { get; set; }

        public ulong ChannelId { get; set; }

        public ulong? GuildId { get; set; }
    }
}
