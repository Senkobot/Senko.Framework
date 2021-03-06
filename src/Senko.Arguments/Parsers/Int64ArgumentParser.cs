﻿using System;

namespace Senko.Arguments.Parsers
{
    public class Int64ArgumentParser : IntArgumentParser<long>
    {
        public Int64ArgumentParser() : base(true)
        {
        }

        protected override bool TryParse(ReadOnlySpan<char> data, out long value)
        {
            return long.TryParse(data, out value);
        }
    }
}