using System;
using System.Collections.Generic;
using System.Linq;
using Senko.Discord;
using Senko.Arguments.Parsers;

namespace Senko.Arguments
{
    public class ArgumentReaderFactory : IArgumentReaderFactory
    {
        private readonly IDiscordClient _client;
        private readonly IServiceProvider _provider;

        public ArgumentReaderFactory(IDiscordClient client, IServiceProvider provider)
        {
            _client = client;
            _provider = provider;
        }

        public IArgumentReader Create(string input, ulong? guildId = null)
        {
            return new ArgumentReader(input.AsMemory(), _client, _provider, guildId);
        }
    }
}
