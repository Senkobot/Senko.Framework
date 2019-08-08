using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.DependencyInjection;
using Miki.Discord;
using Senko.Commands.Managers;
using Senko.Commands.Reflection;
using Senko.Commands.Repositories;
using Senko.Events;
using Senko.Framework;
using Senko.Localizations;
using Senko.Modules.Tests.Data.Repositories;
using Senko.Modules.Tests.Modules;
using Senko.TestFramework;
using Senko.TestFramework.Discord;
using Xunit;

namespace Senko.Commands.Tests.Managers
{
    public class PermissionManagerTest
    {
        private static TestContext CreateContext()
        {
            var services = new EventServiceCollection();

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

            services.AddLogging();
            services.AddOptions();
            services.AddSerializer();
            services.AddCacheClient();
            services.AddLocalizations();
            services.AddCommand();
            services.AddTestClient(data);
            services.AddSingleton<IUserPermissionRepository, MemoryUserPermissionRepository>();
            services.AddSingleton<IRolePermissionRepository, MemoryRolePermissionRepository>();
            services.AddSingleton<IChannelPermissionRepository, MemoryChannelPermissionRepository>();
            services.AddSingleton<IGuildModuleRepository, MemoryGuildModuleRepository>();
            services.AddModule<FooModule>();

            var provider = services.BuildServiceProvider();

            return new TestContext
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
            var context = CreateContext();

            Assert.Equal(new[] { "foo.test" }, context.PermissionManager.Permissions);
        }

        [Fact]
        public async Task TestChannelPermissions()
        {
            var context = CreateContext();
            var manager = context.PermissionManager;
            var channel = context.Channel;

            Assert.True(
                await manager.HasChannelPermissionAsync(channel, "foo.test"),
                "By default the channel should have the permission"
            );

            await manager.RevokeChannelPermissionAsync(channel, "foo.test");

            Assert.False(
                await manager.HasChannelPermissionAsync(channel, "foo.test"), 
                "The permission should be revoked"
            );

            await manager.GrantChannelPermissionAsync(channel, "foo.test");

            Assert.True(
                await manager.HasChannelPermissionAsync(channel, "foo.test"), 
                "The permission be granted"
            );
        }

        [Fact]
        public async Task TestUserPermission()
        {
            var context = CreateContext();
            var manager = context.PermissionManager;
            var user = context.User;

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.test"),
                "By default the user should not have the permission"
            );

            await manager.GrantUserPermissionAsync(user, "foo.test");

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.test"), 
                "The permission be granted"
            );

            await manager.RevokeUserPermissionAsync(user, "foo.test");

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.test"), 
                "The permission should be revoked"
            );
        }

        [Fact]
        public async Task TestRolePermission()
        {
            var context = CreateContext();
            var manager = context.PermissionManager;
            var role = context.Role;
            var user = context.User;
            var guild = context.Guild;
            
            await manager.GrantRolePermissionAsync(guild.Id, role.Id, "foo.test");

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.test"),
                "By default the user should not have the permission"
            );

            await user.AddRoleAsync(role);

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.test"), 
                "The permission be granted"
            );

            await manager.RevokeRolePermissionAsync(guild.Id, role.Id, "foo.test");

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.test"), 
                "The permission should be revoked"
            );

            await manager.GrantUserPermissionAsync(user, "foo.test");

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.test"), 
                "The user permission should override the role permission"
            );
        }

        [Fact]
        public async Task TestEveryonePermission()
        {
            var context = CreateContext();
            var manager = context.PermissionManager;
            var user = context.User;
            var guild = context.Guild;

            await manager.GrantRolePermissionAsync(guild.Id, guild.Id, "foo.test");

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.test"),
                "The user should have the permission since it is set on the everyone role"
            );

            await manager.RevokeRolePermissionAsync(guild.Id, guild.Id, "foo.test");

            Assert.False(
                await manager.HasUserPermissionAsync(user, "foo.test"),
                "The permission should be revoked"
            );

            await manager.GrantUserPermissionAsync(user, "foo.test");

            Assert.True(
                await manager.HasUserPermissionAsync(user, "foo.test"),
                "The user permission should override the role permission"
            );
        }

        private class TestContext
        {
            public IPermissionManager PermissionManager { get; set; }

            public IDiscordGuild Guild { get; set; }

            public IDiscordGuildChannel Channel { get; set; }

            public IDiscordGuildUser User { get; set; }

            public IDiscordRole Role { get; set; }
        }
    }
}
