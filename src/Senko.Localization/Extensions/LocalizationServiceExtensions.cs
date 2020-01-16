using Microsoft.Extensions.DependencyInjection;
using Senko.Framework;
using Senko.Localization.Resources;

namespace Senko.Localization
{
    public static class LevelServiceExtensions
    {
        public static IServiceCollection AddLocalizations(this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizer, StringLocalizer>();
            services.AddHostedService<StringLocalizerHostedService>();
            return services;
        }

        public static IServiceCollection AddStringRepository<T>(this IServiceCollection services)
            where T : class, IStringRepository
        {
            services.AddSingleton<IStringRepository, T>();
            return services;
        }
        public static IServiceCollection AddResourceRepository<T>(this IServiceCollection services)
        {
            services.AddSingleton<IStringRepository>(new ResourceStringRepository(typeof(T).Assembly));
            return services;
        }
    }
}
