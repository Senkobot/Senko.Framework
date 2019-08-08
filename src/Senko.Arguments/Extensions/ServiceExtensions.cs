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
            services.AddSingleton<IArgumentParser, StringArgumentParser>();
            services.AddSingleton<IArgumentParser, RemainingArgumentParser>();
            services.AddSingleton<IArgumentParser>(new DiscordIdArgumentParser(ArgumentType.UserMention));
            services.AddSingleton<IArgumentParser>(new DiscordIdArgumentParser(ArgumentType.RoleMention));
            services.AddSingleton<IArgumentParser>(new DiscordIdArgumentParser(ArgumentType.Channel));
            services.AddSingleton<IArgumentParser>(new Int64ArgumentParser());
            services.AddSingleton<IArgumentParser>(new UInt64ArgumentParser());
            return services;
        }
    }
}
