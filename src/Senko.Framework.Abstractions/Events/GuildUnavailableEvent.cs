namespace Senko.Framework.Events
{
    public class GuildUnavailableEvent : IGuildEvent
    {
        public GuildUnavailableEvent(ulong guildId)
        {
            GuildId = guildId;
        }

        public ulong GuildId { get; }

        ulong? IGuildEvent.GuildId => GuildId;
    }
}
