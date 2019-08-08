using Senko.Discord;

namespace Senko.Framework.Features
{
    public interface IUserFeature
    {
        IDiscordUser User { get; set; }
    }
}
