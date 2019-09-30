using System.Threading.Tasks;
using Senko.Discord;

namespace Senko.Arguments
{
    public interface IArgumentReader
    {
        string ReadUnsafeString(string name = null, bool required = false);

        ValueTask<string> ReadStringAsync(string name = null, bool required = false,
            EscapeType type = EscapeType.Default);

        string ReadUnsafeRemaining(string name = null, bool required = false);

        ValueTask<string> ReadRemainingAsync(string name = null, bool required = false,
            EscapeType type = EscapeType.Default);

        ValueTask<IDiscordUser> ReadUserMentionAsync(string name = null, bool required = false);

        ValueTask<IDiscordGuildUser> ReadGuildUserMentionAsync(string name = null, bool required = false);

        ValueTask<IDiscordRole> ReadRoleMentionAsync(string name = null, bool required = false);

        ValueTask<IDiscordGuildChannel> ReadGuildChannelAsync(string name = null, bool required = false);

        ulong ReadUInt64(string name = null, bool required = false);

        long ReadInt64(string name = null, bool required = false);

        int ReadInt32(string name = null, bool required = false);

        uint ReadUInt32(string name = null, bool required = false);

        void Reset();
    }
}
