using System.Threading.Tasks;
using Senko.Discord;

namespace Senko.Framework
{
    public static class DiscordClientExtensions
    {
        public static ValueTask DeleteMessageAsync(this IDiscordClient client, ulong channelId, ulong messageId)
        {
            return client.ApiClient.DeleteMessageAsync(channelId, messageId);
        }
    }
}
