using Microsoft.Extensions.DependencyInjection;
using Senko.Commands;
using Senko.Framework.Managers;
using Senko.Framework.Repositories;

namespace Senko.Framework
{
    public static class GuildOptionsExtensions
    {
        public static IServiceCollection AddGuildOptions(this IServiceCollection services)
        {
            services.AddSingleton<IGuildOptionsManager, GuildOptionsManager>();
            services.AddSingleton<ICommandValueProvider, GuildOptionsCommandValueProvider>();

            return services;
        }

        public static IServiceCollection AddGuildOptions<TRepository>(this IServiceCollection services)
            where TRepository : class, IGuildOptionRepository
        {
            AddGuildOptions(services);
            services.AddScoped<IGuildOptionRepository, TRepository>();
            return services;
        }
    }
}
