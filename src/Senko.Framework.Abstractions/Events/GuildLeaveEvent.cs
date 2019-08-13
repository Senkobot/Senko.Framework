namespace Senko.Discord.Events
{
    public class GuildLeaveEvent : IGuildEvent
    {
        public GuildLeaveEvent(ulong guildId)
        {
            GuildId = guildId;
        }

        public ulong GuildId { get; }

        ulong? IGuildEvent.GuildId => GuildId;
    }
}
