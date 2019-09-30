using Microsoft.Extensions.DependencyInjection;
using Senko.Commands;
using Senko.Framework.Managers;

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
    }
}
