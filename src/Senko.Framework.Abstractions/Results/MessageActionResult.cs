using System;
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
                        Message.ToMessageArgs()
                    );
                }
                else
                {
                    result = await client.EditMessageAsync(
                        Message.ChannelId,
                        Message.MessageId.Value,
                        Message.ToEditMessageArgs()
                    );
                }

                await Message.InvokeSuccessAsync(
                    new ResponseMessageSuccessArguments(result, Message, client)
                );
            }
            catch (Exception e)
            {
                await Message.InvokeErrorAsync(new ResponseMessageErrorArguments(e, client));
                throw;
            }
        }
    }
}
