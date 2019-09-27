using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Framework.Discord;

namespace Senko.Framework.Results
{
    public struct MessageActionResult : IActionResult
    {
        public MessageActionResult(MessageBuilder message)
        {
            Message = message;
        }
        
        public MessageBuilder Message { get; }
        
        public async ValueTask ExecuteAsync(MessageContext context)
        {
            var client = context.RequestServices.GetRequiredService<IDiscordClient>();

            try
            {
                IDiscordMessage result;

                if (!Message.MessageId.HasValue)
                {
                    result = await client.SendMessageAsync(
                        Message.ChannelId,
                        new MessageArgs
                        {
                            Content = Message.Content,
                            TextToSpeech = Message.IsTTS,
                            Embed = Message.EmbedBuilder?.ToEmbed()
                        }
                    );
                }
                else
                {
                    result = await client.EditMessageAsync(
                        Message.ChannelId,
                        Message.MessageId.Value,
                        new EditMessageArgs
                        {
                            Content = Message.Content,
                            Embed = Message.EmbedBuilder?.ToEmbed()
                        }
                    );
                }

                await Message.InvokeSuccessAsync(
                    new ResponseMessageSuccessArguments(result, Message, client)
                );
            }
            catch (Exception e)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<MessageActionResult>>();

                logger.LogError(e, "An exception occured while sending the response");
                await Message.InvokeErrorAsync(new ResponseMessageErrorArguments(e, client));
            }
        }
    }
}
