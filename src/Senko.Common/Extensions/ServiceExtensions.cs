using System;
using System.Collections.Generic;
using System.Text;
using Foundatio.Caching;
using Foundatio.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Framework.Options;
using StackExchange.Redis;

namespace Senko.Common
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSerializer(this IServiceCollection services, Action<SerializerOptions> configureOptions)
        {
            AddCacheClient(services);
            services.Configure(configureOptions);
            return services;
        }

        public static IServiceCollection AddSerializer(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var options = provider.GetService<IOptions<SerializerOptions>>().Value;

                switch (options.Type.ToLower())
                {
                    case "msgpack":
                        return new MessagePackSerializer();
                    case null:
                        return DefaultSerializer.Instance;
                    default:
                        throw new NotSupportedException($"The cache type {options.Type} is not supported.");
                }
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

                ICacheClient client;

                switch (options.Type.ToLower())
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
                            ConnectionMultiplexer = ConnectionMultiplexer.Connect(options.ConnectionString),
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
    }
}
