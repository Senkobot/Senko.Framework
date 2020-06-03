using Senko.Arguments.Parsers;
using Xunit;

namespace Senko.Arguments.Tests.Parsers
{
    public class DiscordUserIdArgumentParserTest
    {
        private readonly IArgumentParser<DiscordUserId> _parser = new DiscordUserIdArgumentParser();

        [Theory]
        [InlineData("<@114280336535453703>", 114280336535453703u, 21)]
        [InlineData("<@!114280336535453703>", 114280336535453703u, 22)]
        public void TryConsumeTest(string data, ulong result, int length)
        {
            Assert.True(_parser.TryConsume(data, out var value, out var consumedLength));
            Assert.Equal(result, value.Id);
            Assert.Equal(length, consumedLength);
        }

        [Theory]
        [InlineData("<@00000>")]
        [InlineData("<@114280336535453703 ")]
        public void TryConsumeInvalidTest(string data)
        {
            Assert.False(_parser.TryConsume(data, out var value, out var consumedLength));
            Assert.Equal(default, value);
            Assert.Equal(0, consumedLength);
        }
    }
}