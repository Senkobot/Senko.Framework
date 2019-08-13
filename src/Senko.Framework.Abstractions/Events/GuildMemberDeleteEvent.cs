namespace Senko.Discord.Events
{
    public class GuildMemberDeleteEvent : IGuildEvent
    {
        public GuildMemberDeleteEvent(IDiscordGuildUser member)
        {
            Member = member;
        }

        public IDiscordGuildUser Member { get; }

        ulong? IGuildEvent.GuildId => Member.GuildId;
    }
}
