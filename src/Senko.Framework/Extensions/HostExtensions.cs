using System;
using System.Linq;
using System.Reflection;
using Foundatio.Caching;
using Foundatio.Messaging;
using Foundatio.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Senko.Discord;
using Senko.Discord.Gateway;
using Senko.Events;
using Senko.Framework.Events;
using Senko.Framework.Hosting;
using Senko.Framework.Options;
using Senko.Framework.Services;
using Senko.Localization;
using StackExchange.Redis;

namespace Senko.Framework
{
    public static class HostExtensions
    {
        private const string DiscordBotProperty = "SenkoBotRegistered";

        public static IHostBuilder ConfigureDiscordBot(
            this IHostBuilder hostBuilder,
            Action<IBotApplicationBuilder> configure = null)
        {
            return ConfigureDiscordBot(hostBuilder, true, configure);
        }

        internal static IHostBuilder ConfigureDiscordBot(
            this IHostBuilder hostBuilder,
            bool throwOnError,
            Action<IBotApplicationBuilder> configure = null)
        {
            hostBuilder.Properties[DiscordBotProperty] = true;
            
            hostBuilder.ConfigureServices((ctx, services) =>
            {
                AddDefaultServices(services);
                AddOptions(services, ctx.Configuration);

                services.AddHostedService<DiscordBotHostedService>();
                services.AddBotApplicationBuilderFactory(provider =>
                {
                    var builder = new BotApplicationBuilder
                    {
                        ApplicationServices = provider
                    };
                    
                    if (!hostBuilder.Properties.TryGetValue("UseStartup.StartupType", out var value)
                        || !(value is Type startupType))
                    {
                        startupType = null;
                    }

                    if (configure != null)
                    {
                        configure.Invoke(builder);
                    }
                    else if (!InitializeAspNetCore(provider, startupType, builder))
                    {
                        if (throwOnError)
                        {
                            throw new InvalidOperationException("Could not find the ASP.Net Core startup");
                        }
                    }

                    return builder;
                });

                services.PostConfigure<DiscordOptions>(options =>
                {
                    options.Intents = GetIntents(services);
                });
            });

            return hostBuilder;
        }

        private static GatewayIntent GetIntents(IServiceCollection services)
        {
            using var provider = services.BuildServiceProvider();
            var eventManager = provider.GetRequiredService<IEventManager>();

            var intents = GatewayIntent.Guilds
                          | GatewayIntent.DirectMessages
                          | GatewayIntent.GuildMessages
                          | GatewayIntent.GuildMembers;
            
            if (eventManager.IsRegistered<MessageEmojiCreateEvent>()
                || eventManager.IsRegistered<MessageEmojiDeleteEvent>())
            {
                intents |= GatewayIntent.GuildEmojis;
            }
            
            return intents;
        }

        internal static bool IsDiscordBotConfigures(this IHostBuilder hostBuilder)
        {
            return hostBuilder.Properties.TryGetValue(DiscordBotProperty, out var value) && Equals(value, true);
        }
        
        private static bool InitializeAspNetCore(IServiceProvider provider, Type startupType, BotApplicationBuilder builder)
        {
            var startupInterfaceType = Type.GetType("Microsoft.AspNetCore.Hosting.IStartup, Microsoft.AspNetCore.Hosting.Abstractions");
            object startup;

            if (startupInterfaceType != null)
            {
                startup = provider.GetService(startupInterfaceType);

                if (startup != null)
                {
                    startupType = startup.GetType();
                }
            }
            else
            {
                startup = null;
            }

            if (startup == null)
            {
                if (startupType != null)
                {
                    startup = ActivatorUtilities.CreateInstance(provider, startupType);
                }
                else
                {
                    return false;
                }
            }

            var configureMethod = startupType.GetMethod("ConfigureBot");

            if (configureMethod == null)
            {
                return false;
            }
            
            var parameterInfos = configureMethod.GetParameters();
            var parameters = new object[parameterInfos.Length];

            for (var i = 0; i < parameterInfos.Length; i++)
            {
                var parameterType = parameterInfos[i].ParameterType;

                if (parameterType == typeof(IBotApplicationBuilder))
                {
                    parameters[i] = builder;
                }
                else
                {
                    parameters[i] = provider.GetRequiredService(parameterType);
                }
            }

            configureMethod.Invoke(startup, parameters);

            return true;
        }
        
        private static void AddOptions(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config);
            services.AddOptions();

            services.Configure<RedisOptions>(config.GetSection("Redis"));
            services.Configure<CacheOptions>(config.GetSection("Cache"));
            services.Configure<SerializerOptions>(config.GetSection("Serializer"));
            services.Configure<DiscordOptions>(config.GetSection("Discord"));

            var method = typeof(OptionsConfigurationServiceCollectionExtensions).GetMethod("Configure", new[]
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

            if (!services.IsRegistered<IDiscordGateway>())
            {
                services.AddDiscordGateway();
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
