using Senko.Discord;

namespace Senko.Framework.Features
{
    public interface ISelfFeature
    {
        IDiscordUser User { get; set; }
    }
}
