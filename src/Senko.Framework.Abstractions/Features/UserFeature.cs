using Senko.Discord;

namespace Senko.Framework.Features
{
    public struct UserFeature : IUserFeature
    {
        public UserFeature(IDiscordUser user)
        {
            User = user;
        }

        public IDiscordUser User { get; set; }
    }
}
