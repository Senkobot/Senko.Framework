using Microsoft.Extensions.DependencyInjection;
using Senko.Framework.Discord;

namespace Senko.Framework
{
    public static class ResponseExtensions
    {
        public static MessageBuilder AddImage(this MessageResponse response, string url, string title = null, string titleUrl = null, ulong? channelId = null)
        {
            var factory = response.Context.RequestServices.GetRequiredService<IMessageFactory>();
            var message = factory.CreateImage(url, title, titleUrl);

            message.ChannelId = channelId ?? response.Context.Request.ChannelId;

            response.Messages.Add(message);

            return message;
        }

        public static MessageBuilder AddError(this MessageResponse response, string content, ulong? channelId = null)
        {
            var factory = response.Context.RequestServices.GetRequiredService<IMessageFactory>();
            var message = factory.CreateError(content);

            message.ChannelId = channelId ?? response.Context.Request.ChannelId;

            response.Messages.Add(message);

            return message;
        }

        public static MessageBuilder AddSuccess(this MessageResponse response, string content, ulong? channelId = null)
        {
            var factory = response.Context.RequestServices.GetRequiredService<IMessageFactory>();
            var message = factory.CreateSuccess(content);

            message.ChannelId = channelId ?? response.Context.Request.ChannelId;

            response.Messages.Add(message);

            return message;
        }
    }
}
