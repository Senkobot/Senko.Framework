using Microsoft.Extensions.DependencyInjection;
using Senko.Arguments.Parsers;

namespace Senko.Arguments
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddArgumentWithParsers(this IServiceCollection services)
        {
            services.AddArgument();
            services.AddArgumentParsers();
            return services;
        }

        public static IServiceCollection AddArgument(this IServiceCollection services)
        {
            services.AddSingleton<IArgumentReaderFactory, ArgumentReaderFactory>();
            return services;
        }

        public static IServiceCollection AddArgumentParsers(this IServiceCollection services)
        {
            services.AddSingleton<IArgumentParser<string>, StringArgumentParser>();
            services.AddSingleton<IArgumentParser<RemainingString>, RemainingArgumentParser>();
            services.AddSingleton<IArgumentParser<DiscordUserId>, DiscordUserIdArgumentParser>();
            services.AddSingleton<IArgumentParser<DiscordRoleId>, DiscordRoleIdArgumentParser>();
            services.AddSingleton<IArgumentParser<DiscordChannelId>, DiscordChannelIdArgumentParser>();
            services.AddSingleton<IArgumentParser<long>, Int64ArgumentParser>();
            services.AddSingleton<IArgumentParser<ulong>, UInt64ArgumentParser>();
            services.AddSingleton<IArgumentParser<int>, Int32ArgumentParser>();
            services.AddSingleton<IArgumentParser<uint>, UInt32ArgumentParser>();
            return services;
        }
    }
}
