using Senko.Discord;
using Senko.Events;

namespace Senko.Framework.Events
{
    public class UserUpdateEvent : IEvent
    {
        public UserUpdateEvent(IDiscordUser user)
        {
            User = user;
        }

        public IDiscordUser User { get; }
    }
}
