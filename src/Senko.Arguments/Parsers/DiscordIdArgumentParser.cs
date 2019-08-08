using System;
using System.Collections.Generic;
using System.Linq;

namespace Senko.Arguments.Parsers
{
    public class DiscordIdArgumentParser : IArgumentParser
    {
        private static readonly ReadOnlyMemory<char> UserMentionPrefix = "@".AsMemory();
        private static readonly ReadOnlyMemory<char> NicknameMentionPrefix = "@!".AsMemory();
        private static readonly ReadOnlyMemory<char> RoleMentionPrefix = "@&".AsMemory();
        private static readonly ReadOnlyMemory<char> ChannelPrefix = "#".AsMemory();

        private readonly ReadOnlyMemory<char>[] _prefixes;

        public DiscordIdArgumentParser(ArgumentType type)
        {
            _prefixes = GetPrefixes(type).ToArray();
            Type = type;
        }

        public ArgumentType Type { get; }

        public bool TryConsume(ReadOnlySpan<char> data, out Argument argument, out int consumedLength)
        {
            if (data.Length >= 17 && data[0] == '<')
            {
                for (var i = 0; i < _prefixes.Length; i++)
                {
                    var prefix = _prefixes[i];

                    if (!data.Slice(1, prefix.Length).SequenceEqual(prefix.Span))
                    {
                        continue;
                    }

                    var end = data.IndexOf('>');
                    var offset = 1 + prefix.Length;

                    if (end == -1 || !ulong.TryParse(data.Slice(offset, end - offset).ToString(), out var id))
                    {
                        continue;
                    }

                    consumedLength = end + 1;
                    argument = new Argument(Type, id);
                    return true;
                }
            }

            argument = default;
            consumedLength = 0;

            return false;
        }

        private static IEnumerable<ReadOnlyMemory<char>> GetPrefixes(ArgumentType type)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (type)
            {
                case ArgumentType.UserMention:
                    yield return NicknameMentionPrefix;
                    yield return UserMentionPrefix;
                    break;
                case ArgumentType.RoleMention:
                    yield return RoleMentionPrefix;
                    break;
                case ArgumentType.Channel:
                    yield return ChannelPrefix;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, $"The argument type {type} is not supported by {nameof(DiscordIdArgumentParser)}.");
            }
        }
    }
}
