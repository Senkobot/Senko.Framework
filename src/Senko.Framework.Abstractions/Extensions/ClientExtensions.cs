using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Senko.Common;
using Senko.Discord;

namespace Senko.Framework
{
    [Flags]
    public enum EscapeType
    {
        Nothing = 0,
        Global = 1,
        Users = 2,
        Roles = 4,
        Channels = 8,
        Default = Global | Users | Roles,
        Everything = Global | Users | Roles | Channels
    }

    public static class ClientExtensions
    {
        private static readonly Regex MentionRegex = new Regex(@"(?:@(?'name'here|everyone)|<@!?(?'user_id'[0-9]{17,})>|<@&(?'role_id'[0-9]{17,})>|<@&(?'channel_id'[0-9]{17,})>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Task<string> EscapeMentionsAsync(this IDiscordClient client, string value, EscapeType type = EscapeType.Default, ulong? guildId = null)
        {
            if (type == EscapeType.Nothing)
            {
                return Task.FromResult(value);
            }

            return MentionRegex.ReplaceAsync(value, async match =>
            {
                // @here and @everyone
                var name = match.Groups["name"];
                if (name.Success)
                {
                    return type.HasFlag(EscapeType.Global) ? $"@\u200b{name.Value}" : match.Value;
                }

                // @Role
                var roleId = match.Groups["role_id"];
                if (roleId.Success)
                {
                    if (!type.HasFlag(EscapeType.Roles))
                    {
                        return match.Value;
                    }

                    if (!guildId.HasValue)
                    {
                        return "Unknown role";
                    }

                    var role = await client.GetRoleAsync(guildId.Value, ulong.Parse(roleId.Value));

                    return role != null ? $"@\u200b{role.Name}" : "Unknown role";
                }

                // #Channel
                var channelId = match.Groups["channel_id"];
                if (channelId.Success)
                {
                    if (!type.HasFlag(EscapeType.Channels))
                    {
                        return match.Value;
                    }

                    var channel = await client.GetChannelAsync(ulong.Parse(channelId.Value));

                    return channel?.Name ?? "Unknown channel";
                }

                // @User
                var userId = match.Groups["user_id"];

                if (!type.HasFlag(EscapeType.Users))
                {
                    return match.Value;
                }

                IDiscordUser user;

                if (guildId.HasValue)
                {
                    user = await client.GetGuildUserAsync(ulong.Parse(userId.Value), guildId.Value);
                }
                else
                {
                    user = await client.GetUserAsync(ulong.Parse(userId.Value));
                }

                return user?.GetDisplayName() ?? "Unknown user";
            });
        }
    }
}
