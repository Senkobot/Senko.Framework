using System;

namespace Senko.Arguments.Parsers
{
    public class UInt64ArgumentParser : IntArgumentParser<ulong>
    {
        public UInt64ArgumentParser() : base(false)
        {
        }

        protected override bool TryParse(ReadOnlySpan<char> data, out ulong value)
        {
            return ulong.TryParse(data, out value);
        }
    }
}