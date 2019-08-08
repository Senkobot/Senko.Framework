using System;
using System.Collections.Generic;
using System.Text;
using Senko.Events;

namespace Senko.Discord.Events
{
    public interface IGuildEvent : IEvent
    {
        ulong? GuildId { get; }
    }
}
