using System;
using System.Linq;
using System.Threading.Tasks;
using Foundatio.Caching;
using Foundatio.Serializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Framework.Hosting;
using Senko.Framework.Options;
using Senko.Framework.Services;
using StackExchange.Redis;

namespace Senko.Framework
{
    public static class ServiceExtensions
    {
        internal static bool IsRegistered<T>(this IServiceCollection services)
        {
            return services.Any(s => s.ServiceType == typeof(T));
        }

        public static IServiceCollection AddHostedService(this IServiceCollection services, Func<IServiceProvider, Task> func)
        {
            services.AddSingleton<IHostedService>(provider => new FuncService(func, provider));
            return services;
        }

        public static IServiceCollection AddApplicationBuilder(this IServiceCollection services, Action<IApplicationBuilder> factory)
        {
            services.AddApplicationBuilderFactory(() =>
            {
                var builder = new ApplicationBuilder();
                factory(builder);
                return builder;
            });

            return services;
        }
    }
}
