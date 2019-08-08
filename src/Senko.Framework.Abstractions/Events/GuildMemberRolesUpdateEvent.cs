using Senko.Events;

namespace Senko.Discord.Events
{
    public class GuildMemberRolesUpdateEvent : IGuildEvent
    {
        public GuildMemberRolesUpdateEvent(IDiscordGuildUser member)
        {
            Member = member;
        }

        public IDiscordGuildUser Member { get; }

        ulong? IGuildEvent.GuildId => Member.GuildId;
    }
}
