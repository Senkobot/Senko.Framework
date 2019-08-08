using System;
using Microsoft.Extensions.DependencyInjection;
using Senko.Framework.Hosting;

namespace Senko.Framework
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationBuilderFactory(this IServiceCollection services, Func<IApplicationBuilder> factory)
        {
            services.AddSingleton<IApplicationBuilderFactory>(new FuncApplicationBuilderFactory(factory));
            return services;
        }
        
        public static IServiceCollection AddHostedService<T>(this IServiceCollection services) where T : class, IHostedService
        {
            services.AddSingleton<IHostedService, T>();
            return services;
        }

        private class FuncApplicationBuilderFactory : IApplicationBuilderFactory
        {
            private readonly Func<IApplicationBuilder> _factory;

            public FuncApplicationBuilderFactory(Func<IApplicationBuilder> factory)
            {
                _factory = factory;
            }

            public IApplicationBuilder CreateBuilder()
            {
                return _factory();
            }
        }
    }
}
