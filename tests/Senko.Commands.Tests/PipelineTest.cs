﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Arguments;
using Senko.Discord;
using Senko.Framework;
using Senko.Framework.Hosting;
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
        }

        [Fact]
        public async Task TestPendingCommands()
        {
            var data = new TestBotData.Simple();
            var channel = data.Channel;

            var secondUser = new DiscordUser
            {
                Username = "Test",
                Discriminator = "0002"
            };

            data.Users.Add(secondUser);
            data.Guild.Members.Add(secondUser);

            await new TestBotHostBuilder(data, _output)
                .ConfigureService(services =>
                {
                    services.AddArgumentWithParsers();
                    services.AddCommand()
                        .AddModule<GreetModule>();
                })
                .Configure(builder =>
                {
                    builder.UsePendingCommand();
                    builder.UseCommands();
                })
                .RunTestAsync(async client =>
                {
                    await client.SendMessageAsync(channel, data.UserTest, "greet Test");
                    await client.SendMessageAsync(channel, data.UserTest, "1");
                    channel.AssertLastMessage("Hello " + data.UserTest.Username);
                });
        }
    }
}
