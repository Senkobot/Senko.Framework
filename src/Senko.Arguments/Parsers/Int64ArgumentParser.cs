using System;

namespace Senko.Arguments.Parsers
{
    public class Int64ArgumentParser : IArgumentParser
    {
        private readonly bool _signed;

        public Int64ArgumentParser() : this(true)
        {
        }

        internal Int64ArgumentParser(bool signed)
        {
            _signed = signed;
            Type = signed ? ArgumentType.Int64 : ArgumentType.UInt64;
        }

        public ArgumentType Type { get; }

        public bool TryConsume(ReadOnlySpan<char> data, out Argument argument, out int consumedLength)
        {
            argument = default;
            consumedLength = 0;

            // Try to get the value.
            var i = 0;

            if (_signed && data[i] == '-')
            {
                i++;
            }

            for (; i < data.Length; i++)
            {
                var c = data[i];

                if (char.IsWhiteSpace(c))
                {
                    break;
                } 
                
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            
            // Parse the number
            consumedLength = i;

            if (_signed)
            {
                if (!long.TryParse(data.Slice(0, i).ToString(), out var value))
                {
                    return false;
                }
                
                argument = new Argument(ArgumentType.Int64, value);
                return true;
            }
            else
            {
                if (!ulong.TryParse(data.Slice(0, i).ToString(), out var value))
                {
                    return false;
                }
                
                argument = new Argument(ArgumentType.UInt64, value);
                return true;
            }
        }
    }
}
