using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Senko.Discord;
using Senko.Framework.Hosting;
using Senko.Framework.Options;
using Senko.TestFramework.Hosting;
using Xunit.Abstractions;

namespace Senko.TestFramework
{
    public static class BotHostBuilderExtensions
    {
        public static Task RunTestAsync(
            this BotHostBuilder source,
            TestBotData data,
            Func<TestDiscordClient, Task> func)
        {
            return RunTestAsync(source, data, null, func);
        }

        public static async Task RunTestAsync(
            this BotHostBuilder source,
            TestBotData data,
            ITestOutputHelper output,
            Func<TestDiscordClient, Task> func)
        {
            var builder = new BotHostBuilder(source);

            builder.ConfigureService(services =>
            {
                if (output != null)
                {
                    services.AddSingleton(output);
                }

                services.Configure<DebugOptions>(options => { options.ThrowOnMessageException = true; });

                services.TryAddSingleton(data);
                services.TryAddSingleton<IDiscordClient, TestDiscordClient>();
                services.TryAddSingleton<IBotApplication, VoidBotApplication>();
            });

            var host = builder.Build();

            await host.StartAsync();

            try
            {
                var client = (TestDiscordClient)host.CurrentProvider.GetRequiredService<IDiscordClient>();
                await func(client);
            }
            finally
            {
                await host.StopAsync();
            }
        }
    }
}
