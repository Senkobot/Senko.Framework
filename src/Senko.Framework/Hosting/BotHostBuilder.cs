using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundatio.Caching;
using Foundatio.Messaging;
using Foundatio.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Discord;
using Senko.Common;
using Senko.Events;
using Senko.Framework.Options;
using StackExchange.Redis;

namespace Senko.Framework.Hosting
{
    public class BotHostBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly IConfigurationBuilder _configurationBuilder;
        private readonly List<Action<IApplicationBuilder>> _applicationConfigure = new List<Action<IApplicationBuilder>>();

        public BotHostBuilder()
        {
            _configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);
        }

        public BotHostBuilder ConfigureOptions(Action<IConfigurationBuilder> action)
        {
            action(_configurationBuilder);
            return this;
        }

        public BotHostBuilder ConfigureService(Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }

        public BotHostBuilder Configure(Action<IApplicationBuilder> action)
        {
            _applicationConfigure.Add(action);
            return this;
        }

        public BotHost Build()
        {
            var services = new EventServiceCollection();

            foreach (var service in _services)
            {
                services.Add(service);
            }

            AddDefaultServices(services);
            AddOptions(services, _configurationBuilder);
            AddApplication(services, _applicationConfigure);

            return new BotHost(services);
        }
        
        private static void AddApplication(IServiceCollection services, ICollection<Action<IApplicationBuilder>> actions)
        {
            if (services.IsRegistered<IApplicationBuilderFactory>())
            {
                if (actions.Count > 0)
                {
                    throw new InvalidOperationException($"There is already an {nameof(IApplicationBuilderFactory)} registered in the service collection. Unregister the {nameof(IApplicationBuilderFactory)} or remove all the calls to BotHostBuilder.Configure.");
                }

                return;
            }

            services.AddApplicationBuilderFactory(provider =>
            {
                var builder = new ApplicationBuilder
                {
                    ApplicationServices = provider
                };

                foreach (var action in actions)
                {
                    action(builder);
                }

                return builder;
            });
        }

        private static void AddOptions(IServiceCollection services, IConfigurationBuilder builder)
        {
            var config = builder.Build();
            
            services.AddSingleton(config);
            services.AddOptions();

            services.Configure<RedisOptions>(config.GetSection("Redis"));
            services.Configure<CacheOptions>(config.GetSection("Cache"));
            services.Configure<SerializerOptions>(config.GetSection("Serializer"));
            services.Configure<DiscordOptions>(config.GetSection("Discord"));

            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod("Configure", new[]
                {
                    typeof(IServiceCollection),
                    typeof(IConfiguration)
                });

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()))
            {
                var configAttr = type.GetCustomAttribute<ConfigurationAttribute>();

                if (configAttr != null)
                {
                    method.MakeGenericMethod(type)
                        .Invoke(null, new object[]
                        {
                            services, 
                            config.GetSection(configAttr.Path)
                        });
                }
            }
        }

        internal static void AddDefaultServices(IServiceCollection services)
        {
            if (!services.IsRegistered<IConnectionMultiplexer>())
            {
                services.AddRedisConnection();
            }

            if (!services.IsRegistered<IMessageBus>())
            {
                services.AddMessageBus();
            }

            if (!services.IsRegistered<IHybridCacheClient>())
            {
                services.AddSingleton<IHybridCacheClient, HybridCacheClient>();
            }

            if (!services.IsRegistered<IDiscordEventHandler>())
            {
                services.AddSingleton<EventListenerCollection>();
                services.AddSingleton<IDiscordEventHandler, DiscordEventHandler>();
            }

            if (!services.IsRegistered<ILoggerFactory>())
            {
                services.AddLogging();
            }

            if (!services.IsRegistered<IDiscordClient>())
            {
                services.AddDiscord();
            }

            if (!services.IsRegistered<IEventManager>())
            {
                services.AddSingleton<IEventManager, EventManager>();
            }

            if (!services.IsRegistered<IMessageFactory>())
            {
                services.AddSingleton<IMessageFactory, DefaultMessageFactory>();
            }

            if (!services.IsRegistered<IBotApplication>())
            {
                services.AddSingleton<IBotApplication, BotApplication>();
            }

            if (!services.IsRegistered<ISerializer>())
            {
                services.AddSerializer();
            }

            if (!services.IsRegistered<ICacheClient>())
            {
                services.AddCacheClient();
            }

            if (!services.IsRegistered<IMessageContextFactory>())
            {
                services.AddSingleton<IMessageContextFactory, DefaultMessageContextFactory>();
            }

            if (!services.IsRegistered<IMessageContextAccessor>())
            {
                services.AddSingleton<IMessageContextAccessor, MessageContextAccessor>();
            }
        }
    }
}
