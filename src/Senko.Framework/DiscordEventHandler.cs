using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Events;
using Senko.Framework.Events;
using Senko.Framework.Features;
using Senko.Framework.Hosting;

namespace Senko.Framework
{
    public class DiscordEventHandler : IDiscordEventHandler
    {
        private readonly IEventManager _eventManager;
        private readonly IServiceProvider _provider;
        private IDiscordClient _client;

        private readonly MessageDelegate _application;
        private readonly IMessageContextDispatcher _contextDispatcher;
        private readonly IMessageContextFactory _messageContextFactory;
        private readonly IMessageContextAccessor _contextAccessor;
        private readonly ILogger<DiscordEventHandler> _logger;

        public DiscordEventHandler(
            IEventManager eventManager,
            IServiceProvider provider,
            IMessageContextAccessor contextAccessor,
            IMessageContextFactory messageContextFactory,
            IApplicationBuilderFactory builderFactory,
            IMessageContextDispatcher contextDispatcher,
            ILogger<DiscordEventHandler> logger)
        {
            _eventManager = eventManager;
            _provider = provider;
            _contextAccessor = contextAccessor;
            _messageContextFactory = messageContextFactory;
            _contextDispatcher = contextDispatcher;
            _logger = logger;

            var builder = builderFactory.CreateBuilder();
            builder.ApplicationServices = provider;
            _application = builder.Build();
        }

        private IDiscordClient Client => _client ??= _provider.GetRequiredService<IDiscordClient>();

        public Task OnGuildJoin(IDiscordGuild guild)
        {
            return _eventManager.CallAsync(new GuildAvailableEvent(guild));
        }

        public Task OnGuildUpdate(IDiscordGuild guild)
        {
            return _eventManager.CallAsync(new GuildUpdateEvent(guild));
        }

        public Task OnUserUpdate(IDiscordUser user)
        {
            return _eventManager.CallAsync(new UserUpdateEvent(user));
        }

        public Task OnChannelCreate(IDiscordChannel channel)
        {
            return _eventManager.CallAsync(new ChannelCreateEvent(channel));
        }

        public Task OnChannelUpdate(IDiscordChannel channel)
        {
            return _eventManager.CallAsync(new ChannelUpdateEvent(channel));
        }

        public Task OnChannelDelete(IDiscordChannel channel)
        {
            return _eventManager.CallAsync(new ChannelDeleteEvent(channel));
        }

        public Task OnGuildUnavailable(ulong guildId)
        {
            return _eventManager.CallAsync(new GuildUnavailableEvent(guildId));
        }

        public Task OnGuildLeave(ulong guildId)
        {
            return _eventManager.CallAsync(new GuildLeaveEvent(guildId));
        }

        public Task OnGuildMemberDelete(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberDeleteEvent(member));
        }

        public Task OnGuildMemberUpdate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberUpdateEvent(member));
        }

        public Task OnGuildMemberCreate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberCreateEvent(member));
        }

        public Task OnGuildRoleCreate(ulong guildId, IDiscordRole role)
        {
            return _eventManager.CallAsync(new GuildRoleCreateEvent(guildId, role));
        }

        public Task OnGuildRoleUpdate(ulong guildId, IDiscordRole role)
        {
            return _eventManager.CallAsync(new GuildRoleUpdateEvent(guildId, role));
        }

        public Task OnGuildRoleDeleted(ulong guildId, IDiscordRole role)
        {
            return _eventManager.CallAsync(new GuildRoleDeleteEvent(guildId, role));
        }

        public async Task OnMessageCreate(IDiscordMessage message)
        {
            var @event = await _eventManager.CallAsync(new MessageReceivedEvent(message));

            if (@event.IsCancelled)
            {
                return;
            }

            var scope = _provider.CreateScope();
            var features = new FeatureCollection();
            var responseFeature = new MessageResponseFeature();

            features.Set<IServiceProvidersFeature>(new ServiceProvidersFeature(scope.ServiceProvider));
            features.Set<IUserFeature>(new UserFeature(message.Author));
            features.Set<IMessageResponseFeature>(responseFeature);
            features.Set<IMessageRequestFeature>(new MessageRequestFeature
            {
                GuildId = message.GuildId,
                ChannelId = message.ChannelId,
                MessageId = message.Id,
                Message = message.Content
            });

            var context = _messageContextFactory.Create(features);

            try
            {
                _contextAccessor.Context = context;

                await _application(context);
                await _contextDispatcher.DispatchAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception occured while processing the message '{Content}' from {User}.", message.Content, message.Author.Username);
            }
            finally
            {
                scope.Dispose();
                _messageContextFactory.Dispose(context);
            }
        }

        public Task OnMessageUpdate(IDiscordMessage message)
        {
            return _eventManager.CallAsync(new MessageUpdateEvent(message));
        }

        public async Task OnMessageDeleted(ulong channelId, ulong messageId)
        {
            var channel = await Client.GetChannelAsync(channelId);

            await _eventManager.CallAsync(new MessageDeleteEvent(channelId, messageId, (channel as IDiscordGuildChannel)?.GuildId));
        }

        public Task OnMessageEmojiCreated(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return _eventManager.CallAsync(new MessageEmojiCreateEvent(guildId, channelId, messageId, emoji));
        }

        public Task OnMessageEmojiDeleted(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return _eventManager.CallAsync(new MessageEmojiDeleteEvent(guildId, channelId, messageId, emoji));
        }

        public Task OnGuildMemberRolesUpdate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberRolesUpdateEvent(member));
        }
    }
}
