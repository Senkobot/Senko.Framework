namespace Senko.Arguments
{
    public interface IDiscordId
    {
        ulong Id { get; }
        
        DiscordIdType Type { get; }
    }
}