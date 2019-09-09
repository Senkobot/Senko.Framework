using System.Threading.Tasks;
using Senko.Discord;
using Senko.Common;
using Senko.Framework;

namespace Senko.Arguments
{
    public interface IArgumentReader
    {
        string ReadUnsafeString(string name = null, bool required = false);

        Task<string> ReadStringAsync(string name = null, bool required = false, EscapeType type = EscapeType.Default);

        string ReadUnsafeRemaining(string name = null, bool required = false);

        Task<string> ReadRemainingAsync(string name = null, bool required = false, EscapeType type = EscapeType.Default);

        Task<IDiscordUser> ReadUserMentionAsync(string name = null, bool required = false);

        Task<IDiscordGuildUser> ReadGuildUserMentionAsync(string name = null, bool required = false);

        Task<IDiscordRole> ReadRoleMentionAsync(string name = null, bool required = false);

        Task<IDiscordGuildChannel> ReadGuildChannelAsync(string name = null, bool required = false);

        ulong ReadUInt64(string name = null, bool required = false);

        long ReadInt64(string name = null, bool required = false);

        int ReadInt32(string name = null, bool required = false);

        uint ReadUInt32(string name = null, bool required = false);

        void Reset();
    }
}
