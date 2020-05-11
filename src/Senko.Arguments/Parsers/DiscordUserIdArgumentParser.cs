using System;

namespace Senko.Arguments.Parsers
{
    public class DiscordUserIdArgumentParser : DiscordIdArgumentParser<DiscordUserId>
    {
        protected override DiscordUserId GetValue(ulong id)
        {
            return new DiscordUserId(id);
        }

        protected override ReadOnlyMemory<char>[] GetPrefixes()
        {
            return new[]
            {
                "@".AsMemory(),
                "@!".AsMemory()
            };
        }
    }
}