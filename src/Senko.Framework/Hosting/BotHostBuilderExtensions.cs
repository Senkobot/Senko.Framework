using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Senko.Framework.Hosting
{
    public static class BotHostBuilderExtensions
    {
        public static TBuilder ConfigureOptions<TBuilder>(this TBuilder builder, Action<IConfigurationBuilder> action)
            where TBuilder : BotHostBuilder
        {
            action(builder.ConfigurationBuilder);
            return builder;
        }

        public static TBuilder ConfigureService<TBuilder>(this TBuilder builder, Action<IServiceCollection> action)
            where TBuilder : BotHostBuilder
        {
            action(builder.Services);
            return builder;
        }

        public static TBuilder Configure<TBuilder>(this TBuilder builder, Action<IApplicationBuilder> action)
            where TBuilder : BotHostBuilder
        {
            builder.ApplicationConfigure.Add(action);
            return builder;
        }
    }
}
