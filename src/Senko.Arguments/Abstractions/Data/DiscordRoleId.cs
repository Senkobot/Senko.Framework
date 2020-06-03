namespace Senko.Arguments
{
    public readonly struct DiscordRoleId : IDiscordId
    {
        public DiscordRoleId(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; }

        public DiscordIdType Type => DiscordIdType.Role;

        public bool Equals(DiscordRoleId other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is DiscordRoleId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        
        public static implicit operator ulong(DiscordRoleId value)
        {
            return value.Id;
        }
        
        public static implicit operator DiscordRoleId(ulong value)
        {
            return new DiscordRoleId(value);
        }
    }
}