using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Arguments;
using Senko.Discord;
using Senko.Framework;
using Senko.Framework.Hosting;
using Senko.Framework.Repositories;
using Senko.Framework.Repository;
using Senko.TestFramework;
using Senko.TestFramework.Discord;
using Xunit;
using Xunit.Abstractions;

namespace Senko.Commands.Tests
{
    public class GuildOptionsTest
    {
        private readonly ITestOutputHelper _output;

        public GuildOptionsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public class Options
        {
            public string Content { get; set; }
        }

        [CoreModule]
        public class TestModule : ModuleBase
        {
            private readonly IGuildOptions<Options> _options;

            public TestModule(IGuildOptions<Options> options)
            {
                _options = options;
            }

            [Command("get")]
            public void Get()
            {
                Response.AddMessage(_options.Value.Content ?? "No value");
            }

            [Command("set")]
            public async Task Set([Remaining] string content)
            {
                _options.Value.Content = content;
                await _options.StoreAsync();
                Response.AddMessage("Content saved");
            }
        }

        [Fact]
        public async Task TestGuildOptions()
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
                    services.AddCommand()
                        .AddModule<TestModule>();
                    services.AddGuildOptions();
                    services.AddScoped<IGuildOptionRepository, MemoryGuildOptionRepository>();
                })
                .Configure(builder =>
                {
                    builder.UseCommands();
                })
                .RunTestAsync(data, _output, async client =>
                {
                    await client.SendMessageAsync(channel, data.UserTest, "get");
                    channel.AssertLastMessage("No value");
                    await client.SendMessageAsync(channel, data.UserTest, "set Hello");
                    channel.AssertLastMessage("Content saved");
                    await client.SendMessageAsync(channel, data.UserTest, "get");
                    channel.AssertLastMessage("Hello");
                });
        }
    }
}
