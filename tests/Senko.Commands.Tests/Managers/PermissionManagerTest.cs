using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senko.Commands.EfCore;
using Senko.Discord;
using Senko.Commands.Managers;
using Senko.Commands.Tests.Data;
using Senko.Commands.Tests.Modules;
using Senko.Framework;
using Senko.Localization;
using Senko.TestFramework;
using Senko.TestFramework.Discord;
using Xunit;

namespace Senko.Commands.Tests.Managers
{
    public class PermissionManagerTest
    {
        private static TestContext CreateContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var services = new ServiceCollection();

            var user = new DiscordUser
            {
                Username = "Test",
                Discriminator = "0001"
            };

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

            var guildUser = guild.AddMember(user);

            var data = new TestBotData
            {
                Users = { user },
                Guilds = { guild }
            };

            services.AddLocalizations();
            services.AddCommand()
                .AddModule<FooModule>();
            services.AddCommandEfCoreRepositories<TestDbContext>();
            services.AddSingleton<IMessageContextAccessor, MessageContextAccessor>();
            services.AddDbContext<TestDbContext>(builder =>
            {
                builder.UseSqlite(connection);
            });

            var provider = services.BuildTestServiceProvider(data);

            provider.GetRequiredService<TestDbContext>().Database.EnsureCreated();

            return new TestContext(connection)
            {
                PermissionManager = provider.GetRequiredService<IPermissionManager>(),
                Guild = guild,
                Channel = channel,
                User = guildUser,
                Role = role
            };
        }

        [Fact]
        public void TestPropertyPermissions()
        {
            using var context = CreateContext();

            Assert.Contains("foo.foo", context.PermissionManager.Permissions);
            Assert.Contains("foo.bar", context.PermissionManager.Permissions);
        }

        [Fact]
        public async Task TestChannelPermissions()
        {
            using var context = CreateContext();
            var manager = context.PermissionManager;
            var channel = context.Channel;

            Assert.True(
                await manager.HasChannelPermissionAsync(channel, "foo.foo"),
                "By default the channel should have the permission"
            );

            await manager.RevokeChannelPermissionAsync(channel, "foo.foo");

            Assert.False(
                await manager.HasChannelPermissionAsync(channel, "foo.foo"), 
                "The permission should be revoked"
            );

            await manager.GrantChannelPermissionAsync(channel, "foo.foo");

            Assert.True(
                await manager.HasChannelPermissionAsync(channel, "foo.foo"), 
                "The permission be granted"
            );
        }

        [Fact]
        public async Task TestUserPermission()
        {
            using var context = CreateContext();
            var manager = context.PermissionManager;
            var user = context.User;

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.foo"),
                "By default the user should not have the permission"
            );

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.bar"),
                "By default the user should not have the permission"
            );

            await manager.GrantUserPermissionAsync(user, "foo.foo");

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.foo"), 
                "The permission be granted"
            );

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.bar"),
                "The user should not have permissions to foo.bar"
            );

            await manager.GrantUserPermissionAsync(user, "foo.bar");

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.bar"),
                "The permission be granted"
            );

            await manager.RevokeUserPermissionAsync(user, "foo.foo");

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.foo"), 
                "The permission should be revoked"
            );
        }

        [Fact]
        public async Task TestRolePermission()
        {
            using var context = CreateContext();
            var manager = context.PermissionManager;
            var role = context.Role;
            var user = context.User;
            var guild = context.Guild;

            Assert.False(
                await manager.HasRolePermissionAsync(role, guild.Id, "foo.foo"),
                "By default the role should not have the permission"
            );

            await manager.GrantRolePermissionAsync(guild.Id, role.Id, "foo.foo");

            Assert.True(
                await manager.HasRolePermissionAsync(role, guild.Id, "foo.foo"),
                "The permission be granted"
            );

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.foo"),
                "The user should not have the permissions since it doesn't have the role'"
            );

            await user.AddRoleAsync(role);

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.foo"), 
                "The permission be granted"
            );

            await manager.RevokeRolePermissionAsync(guild.Id, role.Id, "foo.foo");

            Assert.False(
                await manager.HasRolePermissionAsync(role, guild.Id, "foo.foo"),
                "The permission should be revoked"
            );

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.foo"), 
                "The permission should be revoked"
            );

            await manager.GrantUserPermissionAsync(user, "foo.foo");

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.foo"), 
                "The user permission should override the role permission"
            );
        }

        [Fact]
        public async Task TestEveryonePermission()
        {
            using var context = CreateContext();
            var manager = context.PermissionManager;
            var user = context.User;
            var guild = context.Guild;

            await manager.GrantRolePermissionAsync(guild.Id, guild.Id, "foo.foo");

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.foo"),
                "The user should have the permission since it is set on the everyone role"
            );

            await manager.RevokeRolePermissionAsync(guild.Id, guild.Id, "foo.foo");

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.foo"),
                "The permission should be revoked"
            );

            await manager.GrantUserPermissionAsync(user, "foo.foo");

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.foo"),
                "The user permission should override the role permission"
            );
        }

        private class TestContext : IDisposable
        {
            private readonly SqliteConnection _connection;

            public TestContext(SqliteConnection connection)
            {
                _connection = connection;
            }

            public IPermissionManager PermissionManager { get; set; }

            public IDiscordGuild Guild { get; set; }

            public IDiscordGuildChannel Channel { get; set; }

            public IDiscordGuildUser User { get; set; }

            public IDiscordRole Role { get; set; }

            public void Dispose()
            {
                _connection?.Dispose();
            }
        }
    }
}
