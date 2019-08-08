using System.Runtime.CompilerServices;
using Senko.Discord;

namespace Senko.Framework
{
    public static class UserExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDisplayName(this IDiscordUser user)
        {
            return (user as IDiscordGuildUser)?.Nickname ?? user.Username;
        }
    }
}
