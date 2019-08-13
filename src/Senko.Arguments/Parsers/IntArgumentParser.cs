using System;

namespace Senko.Arguments.Parsers
{
    public abstract class IntArgumentParser<T> : IArgumentParser
    {
        private readonly bool _signed;
        private readonly ArgumentType _type;

        internal IntArgumentParser(ArgumentType type, bool signed)
        {
            _signed = signed;
            _type = type;
            Type = signed ? ArgumentType.Int64 : ArgumentType.UInt64;
        }

        public ArgumentType Type { get; }

        protected abstract bool TryParse(ReadOnlySpan<char> data, out T value);

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

            if (!TryParse(data.Slice(0, i).ToString(), out var value))
            {
                return false;
            }
                
            argument = new Argument(_type, value);
            return true;
        }
    }
}
