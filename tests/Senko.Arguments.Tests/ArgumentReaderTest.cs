using System.Collections.Generic;
using System.Threading.Tasks;
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
            var user = new DiscordUser
            {
                Username = "Test",
                Discriminator = "0001"
            };
            
            var ambiguousUser1 = new DiscordUser
            {
                Username = "DuplicateName",
                Discriminator = "0001"
            };
            
            var ambiguousUser2 = new DiscordUser
            {
                Username = "DuplicateName",
                Discriminator = "0002"
            };

            var channel = new DiscordGuildTextChannel();
            var role = new DiscordRole();
            var guild = new DiscordGuild
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

            var factory = new ArgumentReaderFactory(
                new TestDiscordClient(data, new VoidDiscordEventHandler()),
                new List<IArgumentParser>
                {
                    new StringArgumentParser(),
                    new RemainingArgumentParser(),
                    new DiscordIdArgumentParser(ArgumentType.UserMention),
                    new DiscordIdArgumentParser(ArgumentType.RoleMention),
                    new DiscordIdArgumentParser(ArgumentType.Channel),
                    new Int64ArgumentParser(),
                    new UInt64ArgumentParser(),
                    new Int32ArgumentParser(),
                    new UInt32ArgumentParser()
                }
            ); 

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
