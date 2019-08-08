using Senko.Arguments.Parsers;
using Xunit;

namespace Senko.Arguments.Tests.Parsers
{
    public class RemainingArgumentParserTest
    {
        private readonly IArgumentParser _argumentParser = new RemainingArgumentParser();

        [Theory]
        [InlineData("Foo", "Foo", 3)]
        [InlineData("Foo Bar", "Foo Bar", 7)]
        [InlineData("'Foo Bar'", "'Foo Bar'", 9)]
        [InlineData("'Foo Bar", "'Foo Bar", 8)]
        public void TryConsumeTest(string data, string expected, int length)
        {
            Assert.True(_argumentParser.TryConsume(data, out var argument, out var consumedLength));
            Assert.Equal(ArgumentType.Remaining, argument.Type);
            Assert.Equal(expected, argument.Value);
            Assert.Equal(length, consumedLength);
        }
    }
}
