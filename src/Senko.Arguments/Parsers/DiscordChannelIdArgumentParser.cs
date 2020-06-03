using System;

namespace Senko.Arguments.Parsers
{
    public class DiscordChannelIdArgumentParser : DiscordIdArgumentParser<DiscordChannelId>
    {
        protected override DiscordChannelId GetValue(ulong id)
        {
            return new DiscordChannelId(id);
        }

        protected override ReadOnlyMemory<char>[] GetPrefixes()
        {
            return new[]
            {
                "#".AsMemory()
            };
        }
    }
}