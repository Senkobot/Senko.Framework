#pragma warning disable 0067

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Discord.Rest;
using Senko.TestFramework.Discord;

namespace Senko.TestFramework
{
    public class TestDiscordClient : IDiscordClient
    {
        private readonly TestBotData _data;

        public TestDiscordClient(TestBotData data, IDiscordEventHandler eventHandler)
        {
            _data = data;
            EventHandler = eventHandler;

            InitCollection(data.Guilds);
            InitCollection(data.Users);
        }

        public IDiscordEventHandler EventHandler { get; }

        public IDiscordGateway Gateway => throw new NotSupportedException();

        public ulong? CurrentUserId { get; set; }

        public IDiscordApiClient ApiClient => throw new NotSupportedException();

        public DiscordSelfUser CurrentUser => _data.CurrentUser;

        public ObservableCollection<IDiscordChannel> Channels  => _data.Channels;

        public ObservableCollection<DiscordUser> Users  => _data.Users;

        public ObservableCollection<DiscordGuild> Guilds  => _data.Guilds;

        private void InitCollection<T>(ObservableCollection<T> collection) where T : class, IDiscordClientContainer
        {
            foreach (var item in collection)
            {
                item.Client = this;
            }

            collection.CollectionChanged += (sender, args) =>
            {
                if (args.Action != NotifyCollectionChangedAction.Add)
                {
                    return;
                }

                foreach (var obj in args.NewItems)
                {
                    if (obj is IDiscordClientContainer container)
                    {
                        container.Client = this;
                    }
                }
            };
        }

        public ValueTask<IDiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, string text, DiscordEmbed embed = null)
        {
            throw new NotImplementedException();
        }

        public ValueTask StartAsync()
        {
            return default;
        }

        public ValueTask StopAsync()
        {
            return default;
        }

        public ValueTask<IDiscordTextChannel> CreateDMAsync(ulong userid)
        {
            var user = Users.FirstOrDefault(u => u.Id == userid);

            return user?.GetDMChannelAsync() ?? default;
        }

        public ValueTask<IDiscordRole> CreateRoleAsync(ulong guildId, CreateRoleArgs args = null)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordPresence> GetUserPresence(ulong userId, ulong? guildId = null)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordPresence> GetUserPresence(ulong userId)
        {
            return Users.FirstOrDefault(u => u.Id == userId)?.GetPresenceAsync() ?? default;
        }

        public async ValueTask<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId)
        {
            var guild = await GetGuildAsync(guildId);

            return await guild.GetRoleAsync(roleId);
        }

        public ValueTask<IEnumerable<IDiscordRole>> GetRolesAsync(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null)
        {
            return new ValueTask<IDiscordChannel>(Channels.FirstOrDefault(c => c.Id == id));
        }

        public ValueTask<T> GetChannelAsync<T>(ulong id, ulong? guildId = null) where T : class, IDiscordChannel
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordSelfUser> GetSelfAsync()
        {
            return new ValueTask<IDiscordSelfUser>(CurrentUser);
        }

        public ValueTask<IDiscordGuild> GetGuildAsync(ulong id)
        {
            return new ValueTask<IDiscordGuild>(Guilds.FirstOrDefault(g => g.Id == id));
        }

        public async ValueTask<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId)
        {
            var guild = await GetGuildAsync(guildId);

            return await guild.GetMemberAsync(id);
        }

        public ValueTask<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId)
        {
            var guild = Guilds.FirstOrDefault(g => g.Id == guildId);

            if (guild == null)
            {
                return new ValueTask<IEnumerable<IDiscordGuildUser>>(Enumerable.Empty<IDiscordGuildUser>());
            }

            return new ValueTask<IEnumerable<IDiscordGuildUser>>(guild.Members);
        }

        public ValueTask<IEnumerable<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordUser> GetUserAsync(ulong id)
        {
            return new ValueTask<IDiscordUser>(Users.FirstOrDefault(u => u.Id == id));
        }

        public ValueTask SetGameAsync(int shardId, DiscordStatus status)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message)
        {
            var channel = Channels.FirstOrDefault(c => c.Id == channelId);

            if (channel == null)
            {
                throw new KeyNotFoundException($"The channel with ID {channelId} was not found.");
            }

            return SendMessageAsync(channel, CurrentUser, message.Content, message.TextToSpeech, message.Embed);
        }

        public async ValueTask<IDiscordMessage> SendMessageAsync(
            IDiscordChannel channel,
            IDiscordUser author,
            string content,
            bool isTTS = false,
            DiscordEmbed embed = null)
        {
            if (!(channel is DiscordTextChannel textChannel))
            {
                throw new InvalidOperationException($"The channel with ID {channel.Id} is not a text channel.");
            }

            var message = new DiscordMessage
            {
                Id = RandomUtil.RandomId(),
                ChannelId = channel.Id,
                Type = DiscordMessageType.DEFAULT,
                Content = content,
                Embed = embed,
                IsTTS = isTTS,
                Author = author,
                GuildId = (channel as IDiscordGuildChannel)?.GuildId,
                Client = this,
                Timestamp = DateTimeOffset.UtcNow
            };

            textChannel.Messages.Add(message);

            await EventHandler.OnMessageCreate(message);

            return message;
        }

        public ValueTask<IDiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs message)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
