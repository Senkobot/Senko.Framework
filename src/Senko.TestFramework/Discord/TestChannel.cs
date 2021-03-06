﻿using System.Threading.Tasks;
using Senko.Discord;

namespace Senko.TestFramework.Discord
{
    public class TestChannel : IDiscordChannel, IDiscordClientContainer, IChangeableSnowflake
    {
        public ulong Id { get; set; }

        public virtual ulong? GuildId => null;

        public bool IsNsfw { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public TestDiscordClient Client { get; set; }

        public ValueTask DeleteAsync()
        {
            IsDeleted = true;

            return default;
        }
    }
}
