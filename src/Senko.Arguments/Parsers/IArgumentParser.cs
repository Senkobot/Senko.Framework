using System;

namespace Senko.Arguments.Parsers
{
    public interface IArgumentParser
    {
        ArgumentType Type { get; }

        bool TryConsume(ReadOnlySpan<char> data, out Argument argument, out int consumedLength);
    }
}
