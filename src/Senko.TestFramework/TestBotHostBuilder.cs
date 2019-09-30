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
    public class TestBotHostBuilder : BotHostBuilder
    {
        private readonly TestBotData _data;
        private readonly ITestOutputHelper _outputHelper;

        public TestBotHostBuilder(ITestOutputHelper outputHelper = null)
            : this(null, outputHelper)
        {
        }

        public TestBotHostBuilder(TestBotData data = null, ITestOutputHelper outputHelper = null)
        {
            _outputHelper = outputHelper;
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
            if (_outputHelper != null)
            {
                services.AddSingleton(_outputHelper);
            }

            services.Configure<DebugOptions>(options =>
            {
                options.ThrowOnMessageException = true;
            });

            services.TryAddSingleton(_data);
            services.TryAddSingleton<IDiscordClient, TestDiscordClient>();
            services.TryAddSingleton<IBotApplication, VoidBotApplication>();

            base.AddDefaultServices(services);
        }
    }
}
