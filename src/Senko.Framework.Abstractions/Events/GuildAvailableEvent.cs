using Senko.Events;

namespace Senko.Discord.Events
{
    public class GuildAvailableEvent : IGuildEvent
    {
        public GuildAvailableEvent(IDiscordGuild guild)
        {
            Guild = guild;
        }

        public IDiscordGuild Guild { get; }

        ulong? IGuildEvent.GuildId => Guild.Id;
    }
}
