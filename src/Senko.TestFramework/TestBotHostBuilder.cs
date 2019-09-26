using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Senko.Discord;
using Senko.Framework.Hosting;
using Senko.TestFramework.Hosting;

namespace Senko.TestFramework
{
    public class TestBotHostBuilder : BotHostBuilder
    {
        private readonly TestBotData _data;

        public TestBotHostBuilder(TestBotData data = null)
        {
            _data = data ?? new TestBotData();
        }

        public async Task RunTestAsync(Func<TestDiscordClient, Task> func)
        {
            var host = Build();

            await host.StartAsync();

            var client = (TestDiscordClient) host.CurrentProvider.GetRequiredService<IDiscordClient>();
            await func(client);

            await host.StopAsync();
        }

        protected override void Configure()
        {
            // Ignore default configuration.
        }

        protected override void AddDefaultServices(IServiceCollection services)
        {
            services.TryAddSingleton(_data);
            services.TryAddSingleton<IDiscordClient, TestDiscordClient>();
            services.TryAddSingleton<IBotApplication, VoidBotApplication>();

            base.AddDefaultServices(services);
        }
    }
}
