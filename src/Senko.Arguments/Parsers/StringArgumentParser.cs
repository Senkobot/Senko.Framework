using System;

namespace Senko.Arguments.Parsers
{
    public class StringArgumentParser : IArgumentParser<string>
    {
        public bool TryConsume(ReadOnlySpan<char> data, out string value, out int consumedLength)
        {
            var isQuoted = IsStringCharacter(data[0], out var endChar);
            int endIndex;

            if (!isQuoted)
            {
                endIndex = data.IndexOfAny(' ', '\n', '\r');

                if (endIndex == -1)
                {
                    consumedLength = data.Length;
                    value = data.ToString();
                    return true;
                }

                consumedLength = endIndex;
                value = data.Slice(0, endIndex).ToString();
                return true;
            }

            endIndex = 1;

            do
            {
                var index = data.Slice(endIndex).IndexOf(endChar);

                if (index == -1)
                {
                    consumedLength = data.Length;
                    value = data.Slice(1, data.Length - 1).ToString();
                    return true;
                }

                endIndex += index;
            } while (data[endIndex - 1] == '\\');

            consumedLength = endIndex + 1;
            value = data.Slice(1, endIndex - 1).ToString();
            return true;
        }
        
        private static bool IsStringCharacter(char startChar, out char endChar)
        {
            switch (startChar)
            {
                case '"':
                case '\'':
                    endChar = startChar;
                    return true;
                case '“':
                    endChar = '”';
                    return true;
                default:
                    endChar = '\0';
                    return false;
            }
        }
    }
}
