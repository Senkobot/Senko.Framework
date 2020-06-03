using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Framework.Discord;

namespace Senko.Framework.Results
{
    public struct DeleteMessageActionResult : IActionResult
    {
        public DeleteMessageActionResult(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }

        public ulong ChannelId { get; }
        
        public ulong MessageId { get; }
        
        public ValueTask ExecuteAsync(MessageContext context)
        {
            var client = context.RequestServices.GetRequiredService<IDiscordClient>();
            
            return  client.DeleteMessageAsync(ChannelId, MessageId);
        }
    }

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
                ValueTask<IDiscordMessage> task;

                if (!Message.MessageId.HasValue)
                {
                    task = client.SendMessageAsync(
                        Message.ChannelId,
                        Message.ToMessageArgs()
                    );
                }
                else
                {
                    task = client.EditMessageAsync(
                        Message.ChannelId,
                        Message.MessageId.Value,
                        Message.ToEditMessageArgs()
                    );
                }

                await Message.InvokeSuccessAsync(
                    new ResponseMessageSuccessArguments(await task, Message, client)
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
