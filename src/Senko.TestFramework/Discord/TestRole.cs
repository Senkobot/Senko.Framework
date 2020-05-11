using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Discord
{
    public class TestRole : IDiscordRole, IChangeableSnowflake
    {
        public ulong Id { get; set; }

        public string Name { get; set; }

        public string RawName => Name;

        public Color Color { get; set; }

        public int Position { get; set; }

        public bool IsManaged { get; set; }

        public bool IsHoisted { get; set; }

        public bool IsMentionable { get; set; }

        public GuildPermission Permissions { get; set; }
    }
}
