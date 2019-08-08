namespace Senko.Framework.Features
{
    public interface IMessageRequestFeature
    {
        string Message { get; set; }

        ulong ChannelId { get; set; }

        ulong? GuildId { get; set; }

        ulong MessageId { get; set; }
    }
}
