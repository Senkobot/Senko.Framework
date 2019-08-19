using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Framework.Discord;

namespace Senko.Framework
{
    public class MessageContextDispatcher : IMessageContextDispatcher
    {
        private readonly IDiscordClient _client;
        private readonly ILogger<MessageContextDispatcher> _logger;
        private readonly IMessageContextFactory _factory;

        public MessageContextDispatcher(
            ILogger<MessageContextDispatcher> logger,
            IDiscordClient client,
            IMessageContextFactory factory
        )
        {
            _logger = logger;
            _client = client;
            _factory = factory;
        }

        public async Task DispatchAsync(Func<MessageContext, Task> func, FeatureCollection features = null)
        {
            var context = _factory.Create(features ?? new FeatureCollection());

            try
            {
                await func(context);
                await DispatchAsync(context);
            }
            finally
            {
                _factory.Dispose(context);
            }
        }

        public async Task DispatchAsync(MessageContext context)
        {
            var messages = context.Response.Messages;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < messages.Count; i++)
            {
                var responseMessage = messages[i];

                try
                {
                    IDiscordMessage result;

                    if (!responseMessage.MessageId.HasValue)
                    {
                        result = await _client.SendMessageAsync(
                            responseMessage.ChannelId,
                            new MessageArgs
                            {
                                Content = responseMessage.Content,
                                TextToSpeech = responseMessage.IsTTS,
                                Embed = responseMessage.EmbedBuilder?.ToEmbed()
                            }
                        );
                    }
                    else
                    {
                        result = await _client.EditMessageAsync(
                            responseMessage.ChannelId,
                            responseMessage.MessageId.Value,
                            new EditMessageArgs
                            {
                                Content = responseMessage.Content,
                                Embed = responseMessage.EmbedBuilder?.ToEmbed()
                            }
                        );
                    }

                    await responseMessage.InvokeSuccessAsync(
                        new ResponseMessageSuccessArguments(result, responseMessage, _client)
                    );
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An exception occured while sending the response");
                    await responseMessage.InvokeErrorAsync(new ResponseMessageErrorArguments(e, _client));
                }
            }
        }
    }
}
