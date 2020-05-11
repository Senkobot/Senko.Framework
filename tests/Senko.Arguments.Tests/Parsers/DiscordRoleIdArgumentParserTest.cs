using Senko.Arguments.Parsers;
using Xunit;

namespace Senko.Arguments.Tests.Parsers
{
    public class DiscordRoleIdArgumentParserTest
    {
        private readonly IArgumentParser<DiscordRoleId> _parser = new DiscordRoleIdArgumentParser();

        [Theory]
        [InlineData("<@&580503697273126912>", 580503697273126912u, 22)]
        public void TryConsumeTest(string data, ulong result, int length)
        {
            Assert.True(_parser.TryConsume(data, out var value, out var consumedLength));
            Assert.Equal(result, value.Id);
            Assert.Equal(length, consumedLength);
        }

        [Theory]
        [InlineData("<@&00000>")]
        [InlineData("<@&580503697273126912 ")]
        public void TryConsumeInvalidTest(string data)
        {
            Assert.False(_parser.TryConsume(data, out var value, out var consumedLength));
            Assert.Equal(default, value);
            Assert.Equal(0, consumedLength);
        }
    }
}