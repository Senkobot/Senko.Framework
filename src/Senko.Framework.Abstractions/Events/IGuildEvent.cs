using Senko.Events;

namespace Senko.Framework.Events
{
    public interface IGuildEvent : IEvent
    {
        ulong? GuildId { get; }
    }
}
