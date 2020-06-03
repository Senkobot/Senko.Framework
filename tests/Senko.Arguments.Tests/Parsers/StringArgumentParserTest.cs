using Senko.Arguments.Parsers;
using Xunit;

namespace Senko.Arguments.Tests.Parsers
{
    public class StringArgumentParserTest
    {
        private readonly IArgumentParser<string> _argumentParser = new StringArgumentParser();

        [Theory]
        [InlineData("Foo", "Foo", 3)]
        [InlineData("Foo Bar", "Foo", 3)]
        [InlineData("'Foo Bar'", "Foo Bar", 9)]
        [InlineData("'Foo Bar", "Foo Bar", 8)]
        public void TryConsumeTest(string data, string expected, int length)
        {
            Assert.True(_argumentParser.TryConsume(data, out var value, out var consumedLength));
            Assert.Equal(expected, value);
            Assert.Equal(length, consumedLength);
        }
    }
}
