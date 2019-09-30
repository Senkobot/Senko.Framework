using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.Framework.Discord
{
    public struct ResponseMessageSuccessArguments
    {
        public ResponseMessageSuccessArguments(IDiscordMessage message, MessageBuilder builder, IDiscordClient client)
        {
            Message = message;
            Builder = builder;
            Client = client;
        }

        public IDiscordMessage Message { get; }

        public MessageBuilder Builder { get; }

        public IDiscordClient Client { get; }
    }

    public struct ResponseMessageErrorArguments
    {
        public ResponseMessageErrorArguments(Exception exception, IDiscordClient client)
        {
            Exception = exception;
            Client = client;
        }

        public Exception Exception { get; }

        public IDiscordClient Client { get; }
    }

    internal struct ResponseMessageSuccessRegister
    {
        public ResponseMessageSuccessRegister(Func<ResponseMessageSuccessArguments, Task> callback, bool combine)
        {
            Callback = callback;
            Combine = combine;
        }

        public Func<ResponseMessageSuccessArguments, Task> Callback { get; }

        public bool Combine { get; }
    }

    public class MessageBuilder
    {
        private readonly IList<ResponseMessageSuccessRegister> _callbacks = new List<ResponseMessageSuccessRegister>();
        private readonly IList<Func<ResponseMessageErrorArguments, Task>> _errorCallbacks = new List<Func<ResponseMessageErrorArguments, Task>>();
        private EmbedBuilder _embed;

        public ulong? MessageId { get; set; }

        public string Content { get; set; }

        public EmbedBuilder EmbedBuilder => _embed ??= new EmbedBuilder();

        public bool HasCallback => _callbacks.Count > 0;

        public bool IsTTS { get; set; }

        public bool IsChanged { get; set; }

        public ulong ChannelId { get; set; }

        public MessageBuilder Then(Func<ResponseMessageSuccessArguments, Task> callback, bool combine = false)
        {
            _callbacks.Add(new ResponseMessageSuccessRegister(callback, combine));
            return this;
        }

        public MessageBuilder Catch(Func<ResponseMessageErrorArguments, Task> callback)
        {
            _errorCallbacks.Add(callback);
            return this;
        }

        public async Task InvokeSuccessAsync(ResponseMessageSuccessArguments args)
        {
            for (var i = 0; i < _callbacks.Count; i++)
            {
                var callback = _callbacks[i];
                IsChanged = false;

                await callback.Callback(args);

                if (callback.Combine)
                {
                    for (var j = i + 1; j < _callbacks.Count && _callbacks[j].Combine; j++)
                    {
                        await _callbacks[j].Callback(args);
                        i++;
                    }
                }

                if (!IsChanged)
                {
                    continue;
                }

                var message = await args.Message.EditAsync(ToEditMessageArgs());

                args = new ResponseMessageSuccessArguments(message, this, args.Client);
            }
        }

        internal MessageArgs ToMessageArgs()
        {
            return new MessageArgs
            {
                Content = Content,
                TextToSpeech = IsTTS,
                Embed = _embed?.ToEmbed()
            };
        }

        internal MessageArgs ToEditMessageArgs()
        {
            return new MessageArgs
            {
                Content = Content,
                Embed = _embed?.ToEmbed()
            };
        }

        public async Task InvokeErrorAsync(ResponseMessageErrorArguments args)
        {
            foreach (var callback in _errorCallbacks)
            {
                await callback(args);
            }
        }
    }
}