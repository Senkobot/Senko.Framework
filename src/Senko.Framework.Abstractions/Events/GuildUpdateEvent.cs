using Senko.Discord;
using Senko.Events;

namespace Senko.Framework.Events
{
    public class GuildUpdateEvent : IEvent
    {
        public GuildUpdateEvent(IDiscordGuild guild)
        {
            Guild = guild;
        }

        public IDiscordGuild Guild { get; }
    }
}
