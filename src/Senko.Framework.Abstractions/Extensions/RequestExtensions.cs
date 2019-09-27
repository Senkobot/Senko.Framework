using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Discord;

namespace Senko.Framework
{
    public static class RequestExtensions
    {
        public const string ChannelItemKey = "senko:channel";
        public const string GuildItemKey = "senko:guild";

        public static async Task<IDiscordGuild> GetGuildAsync(this MessageRequest request)
        {
            if (!request.GuildId.HasValue)
            {
                return null;
            }

            if (request.Context.Items.TryGetValue(GuildItemKey, out var channelObj))
            {
                return (IDiscordGuild)channelObj;
            }

            var client = request.Context.RequestServices.GetService<IDiscordClient>();
            var guild = await client.GetGuildAsync(request.GuildId.Value);
            request.Context.Items[GuildItemKey] = guild;
            return guild;
        }

        public static async Task<IDiscordTextChannel> GetChannelAsync(this MessageRequest request)
        {
            if (request.Context.Items.TryGetValue(ChannelItemKey, out var channelObj))
            {
                return (IDiscordTextChannel) channelObj;
            }
            
            var client = request.Context.RequestServices.GetService<IDiscordClient>();
            var channel = await client.GetChannelAsync(request.ChannelId) as IDiscordTextChannel;
            request.Context.Items[ChannelItemKey] = channel;
            return channel;
        }

        public static ValueTask DeleteMessageAsync(this MessageRequest request)
        {
            var client = request.Context.RequestServices.GetService<IDiscordClient>();

            return client.DeleteMessageAsync(request.ChannelId, request.MessageId);
        }
    }
}
