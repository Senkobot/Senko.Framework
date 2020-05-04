using System;
using Foundatio.Caching;
using Foundatio.Messaging;
using Foundatio.Serializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Framework.Hosting;
using Senko.Framework.Options;
using StackExchange.Redis;

namespace Senko.Framework
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBotApplicationBuilderFactory(this IServiceCollection services, Func<IServiceProvider, IBotApplicationBuilder> factory)
        {
            services.AddSingleton<IBotApplicationBuilderFactory>(provider => new DefaultBotApplicationBuilderFactory(factory, provider));
            return services;
        }

        private class DefaultBotApplicationBuilderFactory : IBotApplicationBuilderFactory
        {
            private readonly Func<IServiceProvider, IBotApplicationBuilder> _factory;
            private readonly IServiceProvider _serviceProvider;

            public DefaultBotApplicationBuilderFactory(Func<IServiceProvider, IBotApplicationBuilder> factory, IServiceProvider serviceProvider)
            {
                _factory = factory;
                _serviceProvider = serviceProvider;
            }

            public IBotApplicationBuilder CreateBuilder()
            {
                return _factory(_serviceProvider);
            }
        }

        public static IServiceCollection AddSerializer<T>(this IServiceCollection services) where T : class, ISerializer
        {
            services.AddSingleton<ISerializer, T>();
            return services;
        }

        public static IServiceCollection AddSerializer(this IServiceCollection services, Action<SerializerOptions> configureOptions)
        {
            AddSerializer(services);
            services.Configure(configureOptions);
            return services;
        }

        public static IServiceCollection AddSerializer(this IServiceCollection services)
        {
            services.AddSingleton<ISerializer>(provider =>
            {
                var options = provider.GetService<IOptions<SerializerOptions>>().Value;

                switch (options.Type.ToLower())
                {
                    case "msgpack":
                        return new MessagePackSerializer();
                    case "json":
                        return new SystemTextJsonSerializer();
                    default:
                        throw new NotSupportedException($"The cache type {options.Type} is not supported.");
                }
            });

            return services;
        }

        public static IServiceCollection AddRedisConnection<T>(this IServiceCollection services) where T : class, IServiceCollection
        {
            services.AddSingleton<IServiceCollection, T>();
            return services;
        }

        public static IServiceCollection AddRedisConnection(this IServiceCollection services, Action<RedisOptions> configureOptions)
        {
            AddRedisConnection(services);
            services.Configure(configureOptions);
            return services;
        }

        public static IServiceCollection AddRedisConnection(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<RedisOptions>>().Value;

                if (string.IsNullOrEmpty(options.ConnectionString))
                {
                    return new InvalidConnectionMultiplexer();
                }

                return ConnectionMultiplexer.Connect(options.ConnectionString);
            });

            return services;
        }

        public static IServiceCollection AddCacheClient<T>(this IServiceCollection services) where T : class, ICacheClient
        {
            services.AddSingleton<ICacheClient, T>();
            return services;
        }

        public static IServiceCollection AddCacheClient(this IServiceCollection services, Action<CacheOptions> configureOptions)
        {
            AddCacheClient(services);
            services.Configure(configureOptions);
            return services;
        }

        public static IServiceCollection AddCacheClient(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<CacheOptions>>().Value;
                var type = options.Type.ToLower();

                ICacheClient client;

                if (type == "auto")
                {
                    var redisOptions = provider.GetRequiredService<IOptions<RedisOptions>>().Value;
                    type = !string.IsNullOrEmpty(redisOptions.ConnectionString) ? "redis" : "memory";
                }

                switch (type)
                {
                    case "inmemory":
                    case "memory":
                        client = new InMemoryCacheClient(new InMemoryCacheClientOptions
                        {
                            LoggerFactory = provider.GetRequiredService<ILoggerFactory>(),
                            Serializer = provider.GetRequiredService<ISerializer>()
                        });
                        break;
                    case "redis":
                        client = new RedisCacheClient(new RedisCacheClientOptions
                        {
                            ConnectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>(),
                            LoggerFactory = provider.GetRequiredService<ILoggerFactory>(),
                            Serializer = provider.GetRequiredService<ISerializer>()
                        });
                        break;
                    default:
                        throw new NotSupportedException($"The cache type {options.Type} is not supported.");
                }

                if (!string.IsNullOrEmpty(options.Prefix))
                {
                    client = new ScopedCacheClient(client, options.Prefix);
                }

                return client;
            });

            return services;
        }

        public static IServiceCollection AddMessageBus<T>(this IServiceCollection services) where T : class, IMessageBus
        {
            services.AddSingleton<IMessageBus, T>();
            return services;
        }

        public static IServiceCollection AddMessageBus(this IServiceCollection services, Action<MessageBusOptions> configureOptions)
        {
            AddMessageBus(services);
            services.Configure(configureOptions);
            return services;
        }

        public static IServiceCollection AddMessageBus(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<MessageBusOptions>>().Value;
                var type = options.Type.ToLower();

                IMessageBus client;

                if (type == "auto")
                {
                    var redisOptions = provider.GetRequiredService<IOptions<RedisOptions>>().Value;
                    type = !string.IsNullOrEmpty(redisOptions.ConnectionString) ? "redis" : "memory";
                }

                switch (type)
                {
                    case "inmemory":
                    case "memory":
                        client = new InMemoryMessageBus(new InMemoryMessageBusOptions
                        {
                            LoggerFactory = provider.GetRequiredService<ILoggerFactory>(),
                            Serializer = provider.GetRequiredService<ISerializer>()
                        });
                        break;
                    case "redis":
                        client = new RedisMessageBus(new RedisMessageBusOptions
                        {
                            Subscriber = provider.GetRequiredService<IConnectionMultiplexer>().GetSubscriber(),
                            LoggerFactory = provider.GetRequiredService<ILoggerFactory>(),
                            Serializer = provider.GetRequiredService<ISerializer>()
                        });
                        break;
                    default:
                        throw new NotSupportedException($"The cache type {options.Type} is not supported.");
                }

                return client;
            });

            return services;
        }
    }
}
