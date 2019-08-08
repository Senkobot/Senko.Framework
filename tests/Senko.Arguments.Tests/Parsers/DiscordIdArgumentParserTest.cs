using System;
using Senko.Arguments.Parsers;
using Xunit;

namespace Senko.Arguments.Tests.Parsers
{
    public class DiscordIdArgumentParserTest
    {
        [Fact]
        public void InvalidTypeTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DiscordIdArgumentParser(ArgumentType.String));
        }

        [Theory]
        [InlineData(ArgumentType.Channel, "<#547466538412933162>", 547466538412933162u, 21)]
        [InlineData(ArgumentType.RoleMention, "<@&580503697273126912>", 580503697273126912u, 22)]
        [InlineData(ArgumentType.UserMention, "<@114280336535453703>", 114280336535453703u, 21)]
        [InlineData(ArgumentType.UserMention, "<@!114280336535453703>", 114280336535453703u, 22)]
        public void TryConsumeTest(ArgumentType type, string data, ulong result, int length)
        {
            var argumentParser = new DiscordIdArgumentParser(type);

            Assert.True(argumentParser.TryConsume(data, out var argument, out var consumedLength));
            Assert.Equal(result, argument.Value);
            Assert.Equal(type, argument.Type);
            Assert.Equal(length, consumedLength);
        }

        [Theory]
        [InlineData(ArgumentType.Channel, "<#00000>")]
        [InlineData(ArgumentType.RoleMention, "<@&00000>")]
        [InlineData(ArgumentType.UserMention, "<@00000>")]
        [InlineData(ArgumentType.Channel, "<#547466538412933162 ")]
        [InlineData(ArgumentType.RoleMention, "<@&580503697273126912 ")]
        [InlineData(ArgumentType.UserMention, "<@114280336535453703 ")]
        public void TryConsumeInvalidTest(ArgumentType type, string data)
        {
            var argumentParser = new DiscordIdArgumentParser(type);

            Assert.False(argumentParser.TryConsume(data, out var argument, out var consumedLength));
            Assert.Null(argument.Value);
            Assert.Equal(0, consumedLength);
        }
    }
}
