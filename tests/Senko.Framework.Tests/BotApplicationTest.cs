using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Senko.Events;
using Senko.Framework.Hosting;
using Senko.Framework.Tests.EventListeners;
using Senko.TestFramework;
using Xunit;
using Xunit.Abstractions;

namespace Senko.Framework.Tests
{
    public class BotApplicationTest
    {
        private readonly ITestOutputHelper _output;

        public BotApplicationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestPipeline()
        {
            var data = new TestBotData.Simple();
            var channel = data.Channel;

            await Host.CreateDefaultBuilder()
                .ConfigureDiscordBot(builder =>
                {
                    builder.Use((context, next) =>
                    {
                        if (context.Request.Message == "Foo")
                        {
                            context.Response.AddMessage("Bar");
                        }

                        return next();
                    });
                })
                .RunTestAsync(data, _output, async client =>
                {
                    await client.SendMessageAsync(channel, data.UserTest, "Foo");
                    channel.AssertLastMessage("Bar");
                });
        }


        [Fact]
        public async Task TestEventListener()
        {
            var data = new TestBotData.Simple();
            var eventListener = new MessageEventListener();

            await Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddEventListener(eventListener);
                })
                .RunTestAsync(data, _output, async client =>
                {
                    await client.SendMessageAsync(data.Channel, data.UserTest, "Foo");
                    Assert.Equal("Foo", eventListener.LastMessage);
                    Assert.Equal("Foo", eventListener.LastMessageTask);
                });
        }
    }
}
