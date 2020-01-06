using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Events;
using Senko.Framework.Events;
using Senko.Framework.Features;
using Senko.Framework.Hosting;
using Senko.Framework.Options;
using Senko.Localization;

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
        private readonly DebugOptions _debugOptions;
        private readonly IStringLocalizer _localizer;

        public DiscordEventHandler(
            IEventManager eventManager,
            IServiceProvider provider,
            IMessageContextAccessor contextAccessor,
            IMessageContextFactory messageContextFactory,
            IBotApplicationBuilderFactory builderFactory,
            IMessageContextDispatcher contextDispatcher,
            ILogger<DiscordEventHandler> logger,
            IOptions<DebugOptions> debugOptions,
            IStringLocalizer localizer)
        {
            _eventManager = eventManager;
            _provider = provider;
            _contextAccessor = contextAccessor;
            _messageContextFactory = messageContextFactory;
            _contextDispatcher = contextDispatcher;
            _logger = logger;
            _localizer = localizer;
            _debugOptions = debugOptions.Value;

            var builder = builderFactory.CreateBuilder();
            builder.ApplicationServices = provider;
            _application = builder.Build();
        }

        private IDiscordClient Client => _client ??= _provider.GetRequiredService<IDiscordClient>();

        public ValueTask OnGuildJoin(IDiscordGuild guild)
        {
            return _eventManager.CallAsync(new GuildAvailableEvent(guild));
        }

        public ValueTask OnGuildUpdate(IDiscordGuild guild)
        {
            return _eventManager.CallAsync(new GuildUpdateEvent(guild));
        }

        public ValueTask OnUserUpdate(IDiscordUser user)
        {
            return _eventManager.CallAsync(new UserUpdateEvent(user));
        }

        public ValueTask OnChannelCreate(IDiscordChannel channel)
        {
            return _eventManager.CallAsync(new ChannelCreateEvent(channel));
        }

        public ValueTask OnChannelUpdate(IDiscordChannel channel)
        {
            return _eventManager.CallAsync(new ChannelUpdateEvent(channel));
        }

        public ValueTask OnChannelDelete(IDiscordChannel channel)
        {
            return _eventManager.CallAsync(new ChannelDeleteEvent(channel));
        }

        public ValueTask OnGuildUnavailable(ulong guildId)
        {
            return _eventManager.CallAsync(new GuildUnavailableEvent(guildId));
        }

        public ValueTask OnGuildLeave(ulong guildId)
        {
            return _eventManager.CallAsync(new GuildLeaveEvent(guildId));
        }

        public ValueTask OnGuildMemberDelete(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberDeleteEvent(member));
        }

        public ValueTask OnGuildMemberUpdate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberUpdateEvent(member));
        }

        public ValueTask OnGuildMemberCreate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberCreateEvent(member));
        }

        public ValueTask OnGuildRoleCreate(ulong guildId, IDiscordRole role)
        {
            return _eventManager.CallAsync(new GuildRoleCreateEvent(guildId, role));
        }

        public ValueTask OnGuildRoleUpdate(ulong guildId, IDiscordRole role)
        {
            return _eventManager.CallAsync(new GuildRoleUpdateEvent(guildId, role));
        }

        public ValueTask OnGuildRoleDeleted(ulong guildId, IDiscordRole role)
        {
            return _eventManager.CallAsync(new GuildRoleDeleteEvent(guildId, role));
        }

        public async ValueTask OnMessageCreate(IDiscordMessage message)
        {
            var @event = new MessageReceivedEvent(message);
            
            await _eventManager.CallAsync(@event);

            if (@event.IsCancelled || string.IsNullOrEmpty(message.Content))
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
                CultureInfo.CurrentCulture = _localizer.DefaultCulture;

                _contextAccessor.Context = context;

                await _application(context);
                await _contextDispatcher.DispatchAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception occured while processing the message '{Content}' from {User}.", message.Content, message.Author.Username);

                if (_debugOptions.ThrowOnMessageException)
                {
                    throw;
                }
            }
            finally
            {
                scope.Dispose();
                _messageContextFactory.Dispose(context);
            }
        }

        public ValueTask OnMessageUpdate(IDiscordMessage message)
        {
            return _eventManager.CallAsync(new MessageUpdateEvent(message));
        }

        public async ValueTask OnMessageDeleted(ulong channelId, ulong messageId)
        {
            var channel = await Client.GetChannelAsync(channelId);

            await _eventManager.CallAsync(new MessageDeleteEvent(channelId, messageId, (channel as IDiscordGuildChannel)?.GuildId));
        }

        public ValueTask OnMessageEmojiCreated(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return _eventManager.CallAsync(new MessageEmojiCreateEvent(guildId, channelId, messageId, emoji));
        }

        public ValueTask OnMessageEmojiDeleted(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return _eventManager.CallAsync(new MessageEmojiDeleteEvent(guildId, channelId, messageId, emoji));
        }

        public ValueTask OnGuildMemberRolesUpdate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberRolesUpdateEvent(member));
        }
    }
}
