using System;

namespace Senko.Arguments.Parsers
{
    public class DiscordRoleIdArgumentParser : DiscordIdArgumentParser<DiscordRoleId>
    {
        protected override DiscordRoleId GetValue(ulong id)
        {
            return new DiscordRoleId(id);
        }

        protected override ReadOnlyMemory<char>[] GetPrefixes()
        {
            return new[]
            {
                "@&".AsMemory()
            };
        }
    }
}