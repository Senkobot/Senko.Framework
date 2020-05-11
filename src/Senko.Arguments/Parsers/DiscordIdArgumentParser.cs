using System;
using System.Collections.Generic;
using System.Linq;

namespace Senko.Arguments.Parsers
{
    public abstract class DiscordIdArgumentParser<T> : IArgumentParser<T>
    {
        private readonly ReadOnlyMemory<char>[] _prefixes;

        public DiscordIdArgumentParser()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            _prefixes = GetPrefixes();
        }

        protected abstract T GetValue(ulong id);

        protected abstract ReadOnlyMemory<char>[] GetPrefixes();

        public bool TryConsume(ReadOnlySpan<char> data, out T value, out int consumedLength)
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
                    value = GetValue(id);
                    return true;
                }
            }

            value = default;
            consumedLength = 0;

            return false;
        }
    }
}
