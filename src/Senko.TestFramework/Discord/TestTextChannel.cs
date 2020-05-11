using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Discord
{
    public class TestTextChannel : TestChannel, IDiscordTextChannel
    {
        public List<TestMessage> Messages { get; set; } = new List<TestMessage>();

        public ValueTask DeleteMessagesAsync(params ulong[] id)
        {
            Messages.RemoveAll(m => id.Contains(m.Id));
            return default;
        }

        public ValueTask DeleteMessagesAsync(params IDiscordMessage[] id)
        {
            return DeleteMessagesAsync(id.Select(m => m.Id).ToArray());
        }

        public IAsyncEnumerable<IDiscordMessage> GetMessagesAsync(int amount = 100)
        {
            return Messages.AsEnumerable()
                .Reverse()
                .Take(amount)
                .ToAsyncEnumerable();
        }

        public ValueTask<IDiscordMessage> GetMessageAsync(ulong id)
        {
            return new ValueTask<IDiscordMessage>(Messages.FirstOrDefault(m => m.Id == id));
        }

        public ValueTask<IDiscordMessage> SendMessageAsync(string content, bool isTTS = false, DiscordEmbed embed = null)
        {
            return Client.SendMessageAsync(Id, new MessageArgs(content, embed, isTTS));
        }

        public ValueTask<IDiscordMessage> SendFileAsync(Stream file, string fileName, string content = null, bool isTTs = false, DiscordEmbed embed = null)
        {
            throw new NotImplementedException();
        }

        public ValueTask TriggerTypingAsync()
        {
            return default;
        }
    }
}
