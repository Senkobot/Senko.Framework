﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Discord;
using Senko.Framework.Events;
using Senko.Framework.Features;

namespace Senko.Framework.Hosting
{
    public class BotApplication : IEventListener, IBotApplication
    {
        private readonly MessageDelegate _application;
        private readonly IMessageContextDispatcher _contextDispatcher;
        private readonly IMessageContextFactory _messageContextFactory;
        private readonly IMessageContextAccessor _contextAccessor;
        private readonly IDiscordClient _client;
        private readonly ILogger<BotApplication> _logger;
        private readonly IServiceProvider _provider;

        public BotApplication(IServiceProvider provider,
            IApplicationBuilderFactory builderFactory,
            IMessageContextFactory messageContextFactory,
            IMessageContextAccessor contextAccessor,
            IDiscordClient client,
            ILogger<BotApplication> logger,
            IMessageContextDispatcher contextDispatcher
        )
        {
            _provider = provider;
            _messageContextFactory = messageContextFactory;
            _contextAccessor = contextAccessor;
            _client = client;
            _logger = logger;
            _contextDispatcher = contextDispatcher;

            var builder = builderFactory.CreateBuilder();
            builder.ApplicationServices = provider;
            _application = builder.Build();
        }

        [EventListener(EventPriority.Low)]
        public async Task HandleMessageAsync(MessageReceivedEvent arg)
        {
            var scope = _provider.CreateScope();
            var features = new FeatureCollection();
            var responseFeature = new MessageResponseFeature();
            
            features.Set<IServiceProvidersFeature>(new ServiceProvidersFeature(scope.ServiceProvider));
            features.Set<IUserFeature>(new UserFeature(arg.Author));
            features.Set<IMessageResponseFeature>(responseFeature);
            features.Set<IMessageRequestFeature>(new MessageRequestFeature
            {
                GuildId = arg.GuildId,
                ChannelId = arg.ChannelId,
                MessageId = arg.Id,
                Message = arg.Content
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
                _logger.LogError(e, "An exception occured while processing the message '{Content}' from {User}.", arg.Content, arg.Author.Username);
            }
            finally
            {
                scope.Dispose();
                _messageContextFactory.Dispose(context);
            }

        }

        public Task StartAsync(CancellationToken token)
        {
            return _client.Gateway.StartAsync();
        }

        public Task StopAsync(CancellationToken token)
        {
            return _client.Gateway.StopAsync();
        }
    }
}
