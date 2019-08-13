namespace Senko.Discord.Events
{
    public class GuildMemberCreateEvent : IGuildEvent
    {
        public GuildMemberCreateEvent(IDiscordGuildUser member)
        {
            Member = member;
        }

        public IDiscordGuildUser Member { get; }

        ulong? IGuildEvent.GuildId => Member.GuildId;
    }
}
