using System;

namespace Senko.Arguments.Parsers
{
    public abstract class IntArgumentParser<T> : IArgumentParser<T>
    {
        private readonly bool _signed;

        internal IntArgumentParser(bool signed)
        {
            _signed = signed;
        }

        protected abstract bool TryParse(ReadOnlySpan<char> data, out T value);

        public bool TryConsume(ReadOnlySpan<char> data, out T value, out int consumedLength)
        {
            value = default;
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
 
            return TryParse(data.Slice(0, i).ToString(), out value);
        }
    }
}
