using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senko.Arguments;
using Senko.Commands.EfCore;
using Senko.Commands.Tests.Data;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Framework;
using Senko.Framework.Hosting;
using Senko.Localization;
using Senko.TestFramework;
using Senko.TestFramework.Discord;
using Xunit;
using Xunit.Abstractions;

namespace Senko.Commands.Tests
{
    public class PipelineTest
    {
        private readonly ITestOutputHelper _output;

        public PipelineTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [CoreModule]
        public class GreetModule : ModuleBase
        {
            [Command("greet")]
            public void Greet(IDiscordUser user)
            {
                Response.AddMessage("Hello " + user.Username);
            }

            [Command("greet")]
            public void Greet()
            {
                Response.AddMessage("Hello stranger");
            }
        }

        [Fact]
        public async Task TestPendingCommands()
        {
            var data = new TestBotData.Simple();
            var channel = data.Channel;

            data.Guild.Members.Add(new DiscordUser
            {
                Username = "Test",
                Discriminator = "0002"
            });

            await new BotHostBuilder()
                .ConfigureService(services =>
                {
                    services.AddArgumentWithParsers();
                    services.AddCommand(builder =>
                    {
                        builder.AddModule<GreetModule>();
                    });
                })
                .Configure(builder =>
                {
                    builder.UsePendingCommand();
                    builder.UseCommands();
                })
                .RunTestAsync(data, _output, async client =>
                {
                    await client.SendMessageAsync(channel, data.UserTest, "greet Test");
                    await client.SendMessageAsync(channel, data.UserTest, "1");
                    channel.AssertLastMessage("Hello " + data.UserTest.Username);
                });
        }

        [Fact]
        public async Task TestPermissions()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var data = new TestBotData.Simple();
            var channel = data.Channel;

            data.Guild.Members[0].UserPermissions = GuildPermission.All;

            await new BotHostBuilder()
                .ConfigureService(services =>
                {
                    services.AddLocalizations();
                    services.AddCommandEfCoreRepositories<TestDbContext>();
                    services.AddArgumentWithParsers();

                    services.AddCommand(builder =>
                    {
                        builder.AddPermissionModule();
                        builder.AddModule<GreetModule>();
                    });

                    services.AddDbContext<TestDbContext>(builder =>
                    {
                        builder.UseSqlite(connection);
                    });

                    services.AddHostedService(provider => provider.GetRequiredService<TestDbContext>().Database.EnsureCreatedAsync());
                })
                .Configure(app =>
                {
                    app.UseCommands();
                })
                .RunTestAsync(data, _output, async client =>
                {
                    // The user should have permissions to the greet command.
                    await client.SendMessageAsync(channel, data.UserTest, "greet Test");
                    channel.AssertLastMessage("Hello " + data.UserTest.Username);

                    // Revoke the command.
                    await client.SendMessageAsync(channel, data.UserTest, "revoke everyone greet.greet");
                    
                    // The command should be revoked.
                    await client.SendMessageAsync(channel, data.UserTest, "greet Test");
                    channel.AssertLastMessage("You don't have permissions to do that.");
                });
        }
    }
}
