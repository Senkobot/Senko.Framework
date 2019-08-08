namespace Senko.Arguments
{
    public interface IArgumentReaderFactory
    {
        IArgumentReader Create(string input, ulong? guildId = null);
    }
}
