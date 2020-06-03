using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senko.Commands.EfCore;
using Senko.Commands.Managers;
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
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var services = new ServiceCollection();

            var channel = new TestGuildTextChannel
            {
                Name = "general"
            };

            var role = new TestRole();
            var guild = new TestGuild
            {
                Channels = { channel },
                Roles = { role }
            };

            var data = new TestBotData
            {
                Guilds = { guild }
            };

            services.AddLocalizations();
            services.AddCommandEfCoreRepositories<TestDbContext>();
            services.AddSingleton<IMessageContextAccessor, MessageContextAccessor>();
            
            services.AddCommand(builder =>
            {
                builder.AddModule<FooModule>();
                builder.AddModule<CoreModule>();
                builder.AddModule<DefaultModule>();
            });

            services.AddDbContext<TestDbContext>(builder =>
            {
                builder.UseSqlite(connection);
            });

            var provider = services.BuildTestServiceProvider(data);

            provider.GetRequiredService<TestDbContext>().Database.EnsureCreated();

            return new TestContext(connection)
            {
                ModuleManager = provider.GetRequiredService<IModuleManager>(),
                Channel = channel,
                Guild = guild
            };
        }

        [Fact]
        public async Task TestDefaultModules()
        {
            using var provider = CreateContext();
            var enabledModules = await provider.ModuleManager.GetEnabledModulesAsync(provider.Guild.Id);

            Assert.DoesNotContain("Foo", enabledModules);
            Assert.Contains("Core", enabledModules);
            Assert.Contains("Default", enabledModules);
        }

        private class TestContext : IDisposable
        {
            private readonly SqliteConnection _connection;

            public TestContext(SqliteConnection connection)
            {
                _connection = connection;
            }

            public IModuleManager ModuleManager { get; set; }

            public IDiscordGuild Guild { get; set; }

            public IDiscordGuildChannel Channel { get; set; }

            public void Dispose()
            {
                _connection?.Dispose();
            }
        }
    }
}
