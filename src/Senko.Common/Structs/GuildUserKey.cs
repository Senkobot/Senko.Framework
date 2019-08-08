namespace Senko.Common.Structs
{
    public struct GuildUserKey
    {
        public GuildUserKey(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }

        public ulong GuildId { get; }

        public ulong UserId { get; }

        public bool Equals(GuildUserKey other)
        {
            return GuildId == other.GuildId && UserId == other.UserId;
        }

        public override bool Equals(object obj)
        {
            return obj is GuildUserKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (GuildId.GetHashCode() * 397) ^ UserId.GetHashCode();
            }
        }
    }
}
