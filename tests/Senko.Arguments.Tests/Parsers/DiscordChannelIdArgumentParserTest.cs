using System;
using Senko.Arguments.Parsers;
using Xunit;

namespace Senko.Arguments.Tests.Parsers
{
    public class DiscordChannelIdArgumentParserTest
    {
        private readonly IArgumentParser<DiscordChannelId> _parser = new DiscordChannelIdArgumentParser();

        [Theory]
        [InlineData("<#547466538412933162>", 547466538412933162u, 21)]
        public void TryConsumeTest(string data, ulong result, int length)
        {
            Assert.True(_parser.TryConsume(data, out var value, out var consumedLength));
            Assert.Equal(result, value.Id);
            Assert.Equal(length, consumedLength);
        }

        [Theory]
        [InlineData("<#00000>")]
        [InlineData("<#547466538412933162 ")]
        public void TryConsumeInvalidTest(string data)
        {
            Assert.False(_parser.TryConsume(data, out var value, out var consumedLength));
            Assert.Equal(default, value);
            Assert.Equal(0, consumedLength);
        }
    }
}
