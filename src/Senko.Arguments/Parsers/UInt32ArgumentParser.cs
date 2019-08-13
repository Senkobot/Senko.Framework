using System;

namespace Senko.Arguments.Parsers
{
    public class UInt32ArgumentParser : IntArgumentParser<uint>
    {
        public UInt32ArgumentParser() : base(ArgumentType.UInt32, false)
        {
        }

        protected override bool TryParse(ReadOnlySpan<char> data, out uint value)
        {
            return uint.TryParse(data, out value);
        }
    }
}