using Microsoft.Extensions.DependencyInjection;
using Senko.Framework.Prefix.Providers;

// ReSharper disable once CheckNamespace
namespace Senko.Framework
{
    public static class PrefixServiceCollectionExtensions
    {
        public static IServiceCollection AddPrefix(this IServiceCollection services, params string[] prefixes)
        {
            services.AddSingleton<IPrefixProvider>(new ListPrefixProvider(prefixes));
            return services;
        }
    }
}
