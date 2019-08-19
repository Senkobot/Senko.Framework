using Senko.Discord;

namespace Senko.Framework.Events
{
    public class GuildMemberUpdateEvent : IGuildEvent
    {
        public GuildMemberUpdateEvent(IDiscordGuildUser member)
        {
            Member = member;
        }

        public IDiscordGuildUser Member { get; }

        ulong? IGuildEvent.GuildId => Member.GuildId;
    }
}