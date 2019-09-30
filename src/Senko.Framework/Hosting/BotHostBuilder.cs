using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundatio.Caching;
using Foundatio.Messaging;
using Foundatio.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Senko.Discord;
using Senko.Events;
using Senko.Framework.Options;
using Senko.Localization;
using StackExchange.Redis;

namespace Senko.Framework.Hosting
{
    public class BotHostBuilder
    {
        internal readonly IServiceCollection Services = new ServiceCollection();
        internal readonly IConfigurationBuilder ConfigurationBuilder;
        internal readonly List<Action<IApplicationBuilder>> ApplicationConfigure = new List<Action<IApplicationBuilder>>();

        public BotHostBuilder()
        {
            ConfigurationBuilder = new ConfigurationBuilder();

            // ReSharper disable once VirtualMemberCallInConstructor
            Configure();
        }

        protected virtual void Configure()
        {
            ConfigurationBuilder.AddJsonFile("appsettings.json", true, true);
        }

        public BotHost Build()
        {
            var services = new ServiceCollection { Services };

            AddDefaultServices(services);
            AddOptions(services, ConfigurationBuilder);
            AddApplication(services, ApplicationConfigure);

            return new BotHost(services);
        }
        
        protected virtual void AddApplication(IServiceCollection services, ICollection<Action<IApplicationBuilder>> actions)
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

        protected virtual void AddOptions(IServiceCollection services, IConfigurationBuilder builder)
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

        protected virtual void AddDefaultServices(IServiceCollection services)
        {
            AddDefaultServicesImpl(services);
        }

        internal static void AddDefaultServicesImpl(IServiceCollection services)
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
                services.AddSingleton<IDiscordEventHandler, DiscordEventHandler>();
            }

            if (!services.IsRegistered<IMessageContextDispatcher>())
            {
                services.AddSingleton<IMessageContextDispatcher, MessageContextDispatcher>();
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

            if (!services.IsRegistered<IStringLocalizer>())
            {
                services.AddSingleton<IStringLocalizer, StringLocalizer>();
            }
        }
    }
}
