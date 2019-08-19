using Senko.Discord;

namespace Senko.Framework.Events
{
    public class GuildRoleUpdateEvent : IGuildEvent
    {
        public GuildRoleUpdateEvent(ulong guildId, IDiscordRole role)
        {
            Role = role;
            GuildId = guildId;
        }

        public ulong GuildId { get; }

        public IDiscordRole Role { get; }

        ulong? IGuildEvent.GuildId => GuildId;
    }
}