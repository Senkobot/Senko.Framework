using System;

namespace Senko.Arguments.Parsers
{
    public interface IArgumentParser<T>
    {
        bool TryConsume(ReadOnlySpan<char> data, out T value, out int consumedLength);
    }
}
