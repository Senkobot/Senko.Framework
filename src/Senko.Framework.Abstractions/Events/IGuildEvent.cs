using Senko.Events;

namespace Senko.Discord.Events
{
    public interface IGuildEvent : IEvent
    {
        ulong? GuildId { get; }
    }
}
