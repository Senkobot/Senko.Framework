using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Events;
using Senko.Framework.Hosting;
using Senko.Framework.Tests.EventListeners;
using Senko.TestFramework;
using Xunit;

namespace Senko.Framework.Tests
{
    public class BotApplicationTest
    {
        [Fact]
        public async Task TestPipeline()
        {
            var data = new TestBotData.Simple();
            var channel = data.Channel;

            await new TestBotHostBuilder(data)
                .Configure(builder =>
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
                .RunTestAsync(async client =>
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

            await new TestBotHostBuilder(data)
                .ConfigureService(services =>
                {
                    services.AddEventListener(eventListener);
                })
                .RunTestAsync(async client =>
                {
                    await client.SendMessageAsync(data.Channel, data.UserTest, "Foo");
                    Assert.Equal("Foo", eventListener.LastMessage);
                    Assert.Equal("Foo", eventListener.LastMessageTask);
                });
        }
    }
}
