using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Senko.Framework.Hosting;

namespace Senko.Framework.AspNetCore
{
    public static class HostExtensions
    {
        public static IHostBuilder ConfigureDiscordBot(this IHostBuilder hostBuilder, Action<IBotHostBuilder> configure = null)
        {
            hostBuilder.ConfigureServices((ctx, services) =>
            {
                var startupType = hostBuilder.Properties["UseStartup.StartupType"] as Type;
                var botHostBuilder = new BotHostBuilder();

                botHostBuilder.BuildServices(services);
                botHostBuilder.BuildOptions(services, ctx.Configuration);

                configure?.Invoke(botHostBuilder);

                services.AddHostedService<BotHostedService>();
                services.AddBotApplicationBuilderFactory(provider =>
                {
                    var startup = provider.GetService(typeof(IStartup));

                    if (startup != null)
                    {
                        startupType = startup.GetType();
                    }
                    
                    var builder = new BotApplicationBuilder
                    {
                        ApplicationServices = provider
                    };

                    if (startupType != null)
                    {
                        var configureMethod = startupType.GetMethod("ConfigureBot");

                        if (configureMethod != null)
                        {
                            if (startup == null)
                            {
                                startup = ActivatorUtilities.CreateInstance(provider, startupType);
                            }

                            var parameterInfos = configureMethod.GetParameters();
                            var parameters = new object[parameterInfos.Length];

                            for (var i = 0; i < parameterInfos.Length; i++)
                            {
                                var type = parameterInfos[i].ParameterType;

                                if (type == typeof(IBotApplicationBuilder))
                                {
                                    parameters[i] = builder;
                                }
                                else
                                {
                                    parameters[i] = provider.GetRequiredService(type);
                                }
                            }

                            configureMethod.Invoke(startup, parameters);
                        }
                    }

                    return builder;
                });
            });

            return hostBuilder;
        }
    }
}
