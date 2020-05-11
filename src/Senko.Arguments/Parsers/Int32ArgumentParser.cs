using System;

namespace Senko.Arguments.Parsers
{
    public class Int32ArgumentParser : IntArgumentParser<int>
    {
        public Int32ArgumentParser() : base(true)
        {
        }

        protected override bool TryParse(ReadOnlySpan<char> data, out int value)
        {
            return int.TryParse(data, out value);
        }
    }
}