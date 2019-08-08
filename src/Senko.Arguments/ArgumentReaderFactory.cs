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
        private readonly IReadOnlyDictionary<ArgumentType, IReadOnlyList<IArgumentParser>> _parsers;

        public ArgumentReaderFactory(IDiscordClient client, IEnumerable<IArgumentParser> parsers)
        {
            _client = client;
            _parsers = parsers.GroupBy(p => p.Type).ToDictionary(g => g.Key, g => (IReadOnlyList<IArgumentParser>)g.ToList());
        }

        public IArgumentReader Create(string input, ulong? guildId = null)
        {
            return new ArgumentReader(input.AsMemory(), _client, _parsers, guildId);
        }
    }
}
