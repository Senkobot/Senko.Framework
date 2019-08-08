using System;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Discord
{
    public class DiscordUser : IDiscordUser, IDiscordClientContainer, IChangeableSnowflake
    {
        public DiscordUser()
        {
            DirectMessageChannel = new DiscordTextChannel();
        }

        public ulong Id { get; set; }

        public string AvatarId { get; set; }

        public string Mention { get; set; }

        public string Username { get; set; }

        public string Discriminator { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsBot { get; set; }

        public DiscordPresence Presence { get; set; }

        public DiscordTextChannel DirectMessageChannel { get; }

        public TestBotClient Client { get; set; }

        public Task<IDiscordPresence> GetPresenceAsync()
        {
            return Task.FromResult<IDiscordPresence>(Presence);
        }

        public Task<IDiscordTextChannel> GetDMChannelAsync()
        {
            return Task.FromResult<IDiscordTextChannel>(DirectMessageChannel);
        }

        public string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256)
        {
            throw new NotImplementedException();
        }

        public DiscordUserPacket Packet => throw new NotSupportedException();

        public override string ToString()
        {
            return Username + '#' + Discriminator;
        }

        protected bool Equals(DiscordUser other)
        {
            return Id == other.Id && string.Equals(AvatarId, other.AvatarId) && string.Equals(Mention, other.Mention) && string.Equals(Username, other.Username) && string.Equals(Discriminator, other.Discriminator) && CreatedAt.Equals(other.CreatedAt) && IsBot == other.IsBot && Equals(Presence, other.Presence) && Equals(DirectMessageChannel, other.DirectMessageChannel);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is DiscordGuildUser dgc) return dgc.User.Equals(this);
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiscordUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (AvatarId != null ? AvatarId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Mention != null ? Mention.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Discriminator != null ? Discriminator.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CreatedAt.GetHashCode();
                hashCode = (hashCode * 397) ^ IsBot.GetHashCode();
                hashCode = (hashCode * 397) ^ (Presence != null ? Presence.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DirectMessageChannel != null ? DirectMessageChannel.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
