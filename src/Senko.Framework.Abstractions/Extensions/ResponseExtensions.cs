using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Senko.Discord.Packets;
using Senko.Framework.Discord;
using Senko.Framework.Results;

namespace Senko.Framework
{
    public static class ResponseExtensions
    {
        public static void Add(this ICollection<IActionResult> items, MessageBuilder builder)
        {
            items.Add(new MessageActionResult(builder));
        }

        public static MessageBuilder AddImage(this MessageResponse response, string url, string title = null, string titleUrl = null, ulong? channelId = null)
        {
            var factory = response.Context.RequestServices.GetRequiredService<IMessageFactory>();
            var message = factory.CreateImage(url, title, titleUrl);

            message.ChannelId = channelId ?? response.Context.Request.ChannelId;

            response.Actions.Add(message);

            return message;
        }

        public static MessageBuilder AddError(this MessageResponse response, string content, ulong? channelId = null)
        {
            var factory = response.Context.RequestServices.GetRequiredService<IMessageFactory>();
            var message = factory.CreateError(content);

            message.ChannelId = channelId ?? response.Context.Request.ChannelId;

            response.Actions.Add(message);

            return message;
        }

        public static MessageBuilder AddSuccess(this MessageResponse response, string content, ulong? channelId = null)
        {
            var factory = response.Context.RequestServices.GetRequiredService<IMessageFactory>();
            var message = factory.CreateSuccess(content);

            message.ChannelId = channelId ?? response.Context.Request.ChannelId;

            response.Actions.Add(message);

            return message;
        }

        public static void React(this MessageResponse response, DiscordEmoji emoji, ulong channelId, ulong messageId)
        {
            response.Actions.Add(new ReactActionResult(channelId, messageId, emoji));
        }

        public static void React(this MessageResponse response, DiscordEmoji emoji)
        {
            var request = response.Context.Request;

            response.Actions.Add(new ReactActionResult(request.ChannelId, request.MessageId, emoji));
        }

        public static void React(this MessageResponse response, string text)
        {
            if (!DiscordEmoji.TryParse(text, out var emoji))
            {
                throw new FormatException("Could not parse the emoji");
            }

            React(response, emoji);
        }

        public static void React(this MessageResponse response, string text, ulong channelId, ulong messageId)
        {
            if (!DiscordEmoji.TryParse(text, out var emoji))
            {
                throw new FormatException("Could not parse the emoji");
            }

            React(response, emoji, channelId, messageId);
        }
    }
}
