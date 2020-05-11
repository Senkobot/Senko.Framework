#pragma warning disable 0067

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Discord.Rest;
using Senko.TestFramework.Discord;
using Xunit.Abstractions;

namespace Senko.TestFramework
{
    public class TestDiscordClient : IDiscordClient
    {
        private readonly TestBotData _data;
        private readonly ITestOutputHelper _outputHelper;

        public TestDiscordClient(TestBotData data, IDiscordEventHandler eventHandler, ITestOutputHelper outputHelper = null)
        {
            _data = data;
            EventHandler = eventHandler;
            _outputHelper = outputHelper;

            InitCollection(data.Guilds);
            InitCollection(data.Users);
        }

        public IDiscordEventHandler EventHandler { get; }

        public IDiscordGateway Gateway => throw new NotSupportedException();

        public ValueTask ModifySelfAsync(UserModifyArgs args)
        {
            throw new NotImplementedException();
        }

        public ulong? CurrentUserId { get; set; }

        public IDiscordApiClient ApiClient => throw new NotSupportedException();

        public TestSelfUser CurrentUser => _data.CurrentUser;

        public ObservableCollection<IDiscordChannel> Channels  => _data.Channels;

        public ObservableCollection<TestUser> Users  => _data.Users;

        public ObservableCollection<TestGuild> Guilds  => _data.Guilds;

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

        public IAsyncEnumerable<IDiscordGuildUser> GetGuildUsersAsync(ulong guildId, IEnumerable<ulong> userIds)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<IEnumerable<IDiscordGuildUserName>> GetGuildMemberNamesAsync(ulong guildId)
        {
            return (await GetGuildMemberNamesAsync(guildId))
                .Select(x => new DiscordGuildUserName(new DiscordGuildMemberPacket
                {
                    Nickname = x.Nickname,
                    User = new DiscordUserPacket
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Discriminator = x.Discriminator
                    }
                }));
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
            if (!(channel is TestTextChannel textChannel))
            {
                throw new InvalidOperationException($"The channel with ID {channel.Id} is not a text channel.");
            }

            var message = new TestMessage
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

            if (_outputHelper != null)
            {
                var sb = new StringBuilder();
                sb.Append(author.Username);
                sb.Append('#');
                sb.Append(author.Discriminator);
                sb.Append(" in ");
                sb.Append(channel.Name);
                sb.Append(": ");

                var length = sb.Length;

                if (!string.IsNullOrEmpty(content))
                {
                    sb.Append(content);
                }

                if (embed != null)
                {
                    const string prefix = "  ";
                    sb.AppendLine();
                    sb.Append(prefix);
                    sb.AppendLine("# Embed");

                    if (!string.IsNullOrEmpty(embed.Description))
                    {
                        foreach (var line in embed.Description.Split('\n'))
                        {
                            sb.Append(prefix);
                            sb.AppendLine(line);
                        }
                    }

                    if (embed.Fields != null)
                    {
                        foreach (var embedField in embed.Fields)
                        {
                            sb.Append("## " + embedField.Title);
                            foreach (var line in embedField.Content.Split('\n'))
                            {
                                sb.Append(prefix);
                                sb.AppendLine(line);
                            }
                        }
                    }
                }

                _outputHelper.WriteLine(sb.ToString());
            }

            await EventHandler.OnMessageCreate(message);

            return message;
        }

        public ValueTask<IDiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs message)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteChannelAsync(ulong channelId)
        {
            throw new NotImplementedException();
        }

        public ValueTask AddGuildBanAsync(ulong guildId, ulong userId, int pruneDays, string reason)
        {
            throw new NotImplementedException();
        }

        public ValueTask RemoveGuildBanAsync(ulong guildId, ulong userId)
        {
            throw new NotImplementedException();
        }

        public ValueTask<int> GetPruneCountAsync(in ulong guildId, in int days)
        {
            throw new NotImplementedException();
        }

        public ValueTask<int?> PruneGuildMembersAsync(ulong guildId, int days, bool computeCount)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteMessagesAsync(ulong id, params ulong[] messageIds)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordMessage> GetMessageAsync(ulong channelId, ulong messageId)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<IDiscordMessage> GetMessagesAsync(ulong channelId, int amount)
        {
            throw new NotImplementedException();
        }

        public ValueTask TriggerTypingAsync(ulong channelId)
        {
            throw new NotImplementedException();
        }

        public ValueTask AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            throw new NotImplementedException();
        }

        public ValueTask KickGuildMemberAsync(ulong guildId, ulong userId, string reason)
        {
            throw new NotImplementedException();
        }

        public ValueTask RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteMessageAsync(ulong channelId, ulong messageId)
        {
            if (!(Channels.FirstOrDefault(c => c.Id == channelId) is TestTextChannel channel))
            {
                throw new KeyNotFoundException($"Channel {channelId} is not found or is not a text channel");
            }

            var message = channel.Messages.FirstOrDefault(m => m.Id == messageId);

            if (message == null)
            {
                throw new KeyNotFoundException($"Message {messageId} is not found");
            }

            message.IsDeleted = true;
            return default;
        }

        public ValueTask DeleteReactionsAsync(ulong channelId, ulong messageId)
        {
            throw new NotImplementedException();
        }

        public ValueTask CreateReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji, ulong userId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
