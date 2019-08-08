using System;

namespace Senko.Arguments.Parsers
{
    public class RemainingArgumentParser : IArgumentParser
    {
        public ArgumentType Type => ArgumentType.Remaining;

        public bool TryConsume(ReadOnlySpan<char> data, out Argument argument, out int consumedLength)
        {
            argument = new Argument(ArgumentType.Remaining, data.ToString());
            consumedLength = data.Length;
            return true;
        }
    }
}
