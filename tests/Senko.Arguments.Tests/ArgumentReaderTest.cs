using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Arguments.Parsers;
using Senko.TestFramework;
using Senko.TestFramework.Discord;
using Senko.TestFramework.Services;
using Xunit;

namespace Senko.Arguments.Tests
{
    public class ArgumentReaderTest
    {
        [Fact]
        public async Task ReadTypesAsync()
        {
            var user = new TestUser
            {
                Username = "Test",
                Discriminator = "0001"
            };
            
            var ambiguousUser1 = new TestUser
            {
                Username = "DuplicateName",
                Discriminator = "0001"
            };
            
            var ambiguousUser2 = new TestUser
            {
                Username = "DuplicateName",
                Discriminator = "0002"
            };

            var channel = new TestGuildTextChannel();
            var role = new TestRole();
            var guild = new TestGuild
            {
                Channels = { channel },
                Roles = { role },
                Members = { user, ambiguousUser1, ambiguousUser2 }
            };

            var data = new TestBotData
            {
                Users = { user, ambiguousUser1, ambiguousUser2 },
                Guilds = { guild }
            };

            var collection = new ServiceCollection();

            collection.AddArgumentWithParsers();
            
            await using var provider = collection.BuildTestServiceProvider(data);
            var factory = provider.GetRequiredService<IArgumentReaderFactory>();

            var reader = factory.Create($"Foo 'Foo Bar' <@{user.Id}> <#{channel.Id}> <@&{role.Id}> -100 100 Test Foo Bar Baz", guild.Id);
            Assert.Null(await reader.ReadUserMentionAsync());
            Assert.Equal("Foo", reader.ReadUnsafeString());
            Assert.Equal("Foo Bar", reader.ReadUnsafeString());
            Assert.Equal(user, await reader.ReadUserMentionAsync());
            Assert.Equal(channel, await reader.ReadGuildChannelAsync());
            Assert.Equal(role, await reader.ReadRoleMentionAsync());
            Assert.Equal(-100, reader.ReadInt64());
            Assert.Equal(100u, reader.ReadUInt64());
            Assert.Equal(user, await reader.ReadUserMentionAsync());
            Assert.Equal("Foo Bar Baz", await reader.ReadRemainingAsync());
            Assert.Null(reader.ReadUnsafeString());
            reader.Reset();
            Assert.Equal("Foo", reader.ReadUnsafeString());

            reader = factory.Create("DuplicateName", guild.Id);

            await Assert.ThrowsAsync<AmbiguousArgumentMatchException>(async () => await reader.ReadUserMentionAsync());
        }
    }
}
