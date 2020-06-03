namespace Senko.Arguments
{
    public readonly struct DiscordUserId : IDiscordId
    {
        public DiscordUserId(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; }

        public DiscordIdType Type => DiscordIdType.User;

        public bool Equals(DiscordUserId other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is DiscordUserId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        
        public static implicit operator ulong(DiscordUserId value)
        {
            return value.Id;
        }
        
        public static implicit operator DiscordUserId(ulong value)
        {
            return new DiscordUserId(value);
        }
    }
}