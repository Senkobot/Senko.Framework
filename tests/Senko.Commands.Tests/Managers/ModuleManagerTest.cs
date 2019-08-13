using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senko.Commands.EfCore;
using Senko.Commands.Managers;
using Senko.Commands.Repositories;
using Senko.Commands.Tests.Data;
using Senko.Commands.Tests.Modules;
using Senko.Discord;
using Senko.Framework;
using Senko.Localization;
using Senko.TestFramework;
using Senko.TestFramework.Discord;
using Xunit;

namespace Senko.Commands.Tests.Managers
{
    public class ModuleManagerTest
    {
        private static TestContext CreateContext()
        {
            var services = new EventServiceCollection();

            var channel = new DiscordGuildTextChannel
            {
                Name = "general"
            };

            var role = new DiscordRole();
            var guild = new DiscordGuild
            {
                Channels = { channel },
                Roles = { role }
            };

            var data = new TestBotData
            {
                Guilds = { guild }
            };

            services.AddLocalizations();
            services.AddCommand();
            services.AddCommandEfCoreRepositories<TestDbContext>();
            services.AddModule<FooModule>();
            services.AddModule<CoreModule>();
            services.AddModule<DefaultModule>();
            services.AddSingleton<IMessageContextAccessor, MessageContextAccessor>();

            services.AddDbContext<TestDbContext>(builder =>
            {
                builder.UseInMemoryDatabase("senko");
            });

            var provide = services.BuildTestServiceProvider(data);

            return new TestContext
            {
                ModuleManager = provide.GetRequiredService<IModuleManager>(),
                Channel = channel,
                Guild = guild
            };
        }

        [Fact]
        public async Task TestDefaultModules()
        {
            var provider = CreateContext();
            var enabledModules = await provider.ModuleManager.GetEnabledModulesAsync(provider.Guild.Id);

            Assert.DoesNotContain("Foo", enabledModules);
            Assert.Contains("Core", enabledModules);
            Assert.Contains("Default", enabledModules);
        }

        private class TestContext
        {
            public IModuleManager ModuleManager { get; set; }

            public IDiscordGuild Guild { get; set; }

            public IDiscordGuildChannel Channel { get; set; }
        }
    }
}
