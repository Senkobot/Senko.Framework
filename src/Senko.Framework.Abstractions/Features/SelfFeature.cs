using Senko.Discord;

namespace Senko.Framework.Features
{
    public struct SelfFeature : ISelfFeature
    {
        public SelfFeature(IDiscordUser user)
        {
            User = user;
        }

        public IDiscordUser User { get; set; }
    }
}
