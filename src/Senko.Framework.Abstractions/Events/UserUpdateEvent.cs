using Senko.Events;

namespace Senko.Discord.Events
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
