using System;
using System.Threading.Tasks;
using Senko.Discord;

namespace Senko.TestFramework.Discord
{
    public class DiscordChannel : IDiscordChannel, IDiscordClientContainer, IChangeableSnowflake
    {
        public ulong Id { get; set; }

        public bool IsNsfw { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public TestDiscordClient Client { get; set; }

        public Task DeleteAsync()
        {
            IsDeleted = true;

            return Task.CompletedTask;
        }
    }
}
