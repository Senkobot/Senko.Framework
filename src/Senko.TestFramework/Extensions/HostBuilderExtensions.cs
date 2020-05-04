using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Senko.Discord;
using Senko.Framework;
using Senko.Framework.Hosting;
using Senko.Framework.Options;
using Senko.TestFramework.Hosting;
using Xunit.Abstractions;

namespace Senko.TestFramework
{
    public static class HostBuilderExtensions
    {
        public static Task RunTestAsync(
            this IHostBuilder source,
            TestBotData data,
            Func<TestDiscordClient, Task> func)
        {
            return RunTestAsync(source, data, null, func);
        }

        public static async Task RunTestAsync(
            this IHostBuilder source,
            TestBotData data,
            ITestOutputHelper output,
            Func<TestDiscordClient, Task> func)
        {
            if (!source.IsDiscordBotConfigures())
            {
                source.ConfigureDiscordBot(false);
            }
            
            source.ConfigureServices(services =>
            {
                if (output != null)
                {
                    services.AddSingleton(output);
                }

                services.Configure<DebugOptions>(options =>
                {
                    options.ThrowOnMessageException = true;
                });

                services.AddSingleton(data);
                services.AddSingleton<IDiscordClient, TestDiscordClient>();
                services.AddSingleton<IBotApplication, VoidBotApplication>();
            });

            var host = source.Build();

            await host.StartAsync();

            try
            {
                var client = (TestDiscordClient)host.Services.GetRequiredService<IDiscordClient>();
                await func(client);
            }
            finally
            {
                await host.StopAsync();
            }
        }
    }
}
