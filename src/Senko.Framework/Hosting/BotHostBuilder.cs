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
    public interface IBotHostBuilder
    {
        BotHostBuilder ConfigureOptions(Action<IConfigurationBuilder> action);
        BotHostBuilder ConfigureService(Action<IServiceCollection> action);
        BotHostBuilder Configure(Action<IBotApplicationBuilder> action);
        IBotHost Build();
    }

    public class BotHostBuilder : IBotHostBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IConfigurationBuilder _configurationBuilder;
        private readonly List<Action<IBotApplicationBuilder>> _applicationConfigure;

        public BotHostBuilder()
        {
            _services = new ServiceCollection();
            _configurationBuilder = new ConfigurationBuilder();
            _applicationConfigure = new List<Action<IBotApplicationBuilder>>();
        }

        public BotHostBuilder(BotHostBuilder source)
            : this()
        {
            _services.Add(source._services);
            _applicationConfigure.AddRange(source._applicationConfigure);

            foreach (var kv in source._configurationBuilder.Properties)
            {
                _configurationBuilder.Properties.Add(kv);
            }

            foreach (var item in source._configurationBuilder.Sources)
            {
                _configurationBuilder.Sources.Add(item);
            }
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

        public BotHostBuilder Configure(Action<IBotApplicationBuilder> action)
        {
            _applicationConfigure.Add(action);
            return this;
        }

        public IBotHost Build()
        {
            var services = new ServiceCollection { _services };
            BuildOptions(services, _configurationBuilder.Build());
            BuildServices(services);
            return new BotHost(services);
        }

        public void BuildServices(IServiceCollection services)
        {
            AddDefaultServices(services);
            AddApplication(services, _applicationConfigure);
        }
        
        protected virtual void AddApplication(IServiceCollection services, ICollection<Action<IBotApplicationBuilder>> actions)
        {
            if (services.IsRegistered<IBotApplicationBuilderFactory>())
            {
                if (actions.Count > 0)
                {
                    throw new InvalidOperationException($"There is already an {nameof(IBotApplicationBuilderFactory)} registered in the service collection. Unregister the {nameof(IBotApplicationBuilderFactory)} or remove all the calls to BotHostBuilder.Configure.");
                }

                return;
            }

            services.AddBotApplicationBuilderFactory(provider =>
            {
                var builder = new BotApplicationBuilder
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

        public virtual void BuildOptions(IServiceCollection services, IConfiguration config)
        {
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
                services.AddLocalizations();
            }
        }
    }
}
