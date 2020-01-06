using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Senko.Framework.Hosting;
using Senko.Framework.Services;

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

        public static IServiceCollection AddApplicationBuilder(this IServiceCollection services, Action<IBotApplicationBuilder> factory)
        {
            services.AddBotApplicationBuilderFactory(provider =>
            {
                var builder = new BotApplicationBuilder
                {
                    ApplicationServices = provider
                };

                factory(builder);

                return builder;
            });

            return services;
        }
    }
}
