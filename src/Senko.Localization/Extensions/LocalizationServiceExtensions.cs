using Microsoft.Extensions.DependencyInjection;

namespace Senko.Localization
{
    public static class LevelServiceExtensions
    {
        public static IServiceCollection AddLocalizations(this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizer, StringLocalizer>();
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
