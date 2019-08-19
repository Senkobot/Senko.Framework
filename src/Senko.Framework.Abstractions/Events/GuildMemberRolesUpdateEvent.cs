using Senko.Discord;

namespace Senko.Framework.Events
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
