using Senko.Arguments.Parsers;
using Xunit;

namespace Senko.Arguments.Tests.Parsers
{
    public class Int64ArgumentParserTest
    {
        private readonly IArgumentParser _signedArgumentParser = new Int64ArgumentParser();
        private readonly IArgumentParser _unsignedArgumentParser = new UInt64ArgumentParser();

        [Theory]
        [InlineData("1", 1, 1)]
        [InlineData("-1", -1, 2)]
        [InlineData("5000", 5000, 4)]
        [InlineData("100 Test", 100, 3)]
        public void TryConsumeSignedTest(string data, long expected, int length)
        {
            Assert.True(_signedArgumentParser.TryConsume(data, out var argument, out var consumedLength));
            Assert.Equal(ArgumentType.Int64, argument.Type);
            Assert.Equal(expected, argument.Value);
            Assert.Equal(length, consumedLength);
        }

        [Theory]
        [InlineData("1", 1, 1)]
        [InlineData("5000", 5000, 4)]
        [InlineData("100 Test", 100, 3)]
        public void TryConsumeUnsignedTest(string data, ulong expected, int length)
        {
            Assert.True(_unsignedArgumentParser.TryConsume(data, out var argument, out var consumedLength));
            Assert.Equal(ArgumentType.UInt64, argument.Type);
            Assert.Equal(expected, argument.Value);
            Assert.Equal(length, consumedLength);
        }
    }
}
