using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Discord
{
    public class DiscordTextChannel : DiscordChannel, IDiscordTextChannel
    {
        public List<DiscordMessage> Messages { get; set; } = new List<DiscordMessage>();

        public Task DeleteMessagesAsync(params ulong[] id)
        {
            Messages.RemoveAll(m => id.Contains(m.Id));
            return Task.CompletedTask;
        }

        public Task DeleteMessagesAsync(params IDiscordMessage[] id)
        {
            return DeleteMessagesAsync(id.Select(m => m.Id).ToArray());
        }

        public Task<IEnumerable<IDiscordMessage>> GetMessagesAsync(int amount = 100)
        {
            return Task.FromResult<IEnumerable<IDiscordMessage>>(Messages.AsEnumerable().Reverse().Take(100));
        }

        public Task<IDiscordMessage> GetMessageAsync(ulong id)
        {
            return Task.FromResult<IDiscordMessage>(Messages.FirstOrDefault(m => m.Id == id));
        }

        public Task<IDiscordMessage> SendMessageAsync(string content, bool isTTS = false, DiscordEmbed embed = null)
        {
            return Client.SendMessageAsync(Id, new MessageArgs(content, embed, isTTS));
        }

        public Task<IDiscordMessage> SendFileAsync(Stream file, string fileName, string content = null, bool isTTs = false, DiscordEmbed embed = null)
        {
            throw new NotImplementedException();
        }

        public Task TriggerTypingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
