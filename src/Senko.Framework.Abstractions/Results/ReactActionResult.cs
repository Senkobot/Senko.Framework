using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.Framework.Results
{
    public struct ReactActionResult : IActionResult
    {
        public ReactActionResult(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            ChannelId = channelId;
            MessageId = messageId;
            Emoji = emoji;
        }

        private ulong ChannelId { get; }
        
        private ulong MessageId { get; }
        
        private DiscordEmoji Emoji { get; }

        public ValueTask ExecuteAsync(MessageContext context)
        {
            var client = context.RequestServices.GetRequiredService<IDiscordClient>();

            return client.CreateReactionAsync(ChannelId, MessageId, Emoji);
        }
    }
}
