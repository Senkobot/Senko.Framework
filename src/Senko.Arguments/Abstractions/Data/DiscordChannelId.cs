namespace Senko.Arguments
{
    public readonly struct DiscordChannelId : IDiscordId
    {
        public DiscordChannelId(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; }
        
        public DiscordIdType Type => DiscordIdType.Channel;

        public bool Equals(DiscordChannelId other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is DiscordChannelId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        
        public static implicit operator ulong(DiscordChannelId value)
        {
            return value.Id;
        }
        
        public static implicit operator DiscordChannelId(ulong value)
        {
            return new DiscordChannelId(value);
        }
    }
}