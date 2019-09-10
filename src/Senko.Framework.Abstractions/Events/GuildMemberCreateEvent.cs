using Senko.Discord;

namespace Senko.Framework.Events
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
