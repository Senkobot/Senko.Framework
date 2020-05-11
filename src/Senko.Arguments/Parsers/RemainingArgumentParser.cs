using System;

namespace Senko.Arguments.Parsers
{
    public class RemainingArgumentParser : IArgumentParser<RemainingString>
    {
        public bool TryConsume(ReadOnlySpan<char> data, out RemainingString value, out int consumedLength)
        {
            value = data.ToString();
            consumedLength = data.Length;
            return true;
        }
    }
}
