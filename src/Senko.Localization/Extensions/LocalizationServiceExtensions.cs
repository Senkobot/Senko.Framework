using Microsoft.Extensions.DependencyInjection;
using Senko.Framework;

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
    }
}
