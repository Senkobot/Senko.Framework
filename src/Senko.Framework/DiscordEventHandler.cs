using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Events;
using Senko.Events;

namespace Senko.Framework
{
    public class DiscordEventHandler : IDiscordEventHandler
    {
        private readonly IEventManager _eventManager;

        public DiscordEventHandler(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public Task OnGuildJoin(IDiscordGuild guild)
        {
            return _eventManager.CallAsync(new GuildAvailableEvent(guild));
        }

        public Task OnUserUpdate(IDiscordUser user)
        {
            return _eventManager.CallAsync(new UserUpdateEvent(user));
        }

        public Task OnGuildUnavailable(ulong guildId)
        {
            return _eventManager.CallAsync(new GuildUnavailableEvent(guildId));
        }

        public Task OnGuildLeave(ulong guildId)
        {
            return _eventManager.CallAsync(new GuildLeaveEvent(guildId));
        }

        public Task OnGuildMemberDelete(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberDeleteEvent(member));
        }

        public Task OnGuildMemberCreate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberCreateEvent(member));
        }

        public Task OnMessageCreate(IDiscordMessage message)
        {
            return _eventManager.CallAsync(new MessageReceivedEvent(message));
        }

        public Task OnMessageUpdate(IDiscordMessage message)
        {
            return _eventManager.CallAsync(new MessageUpdateEvent(message));
        }

        public Task OnGuildMemberRolesUpdate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberRolesUpdateEvent(member));
        }
    }
}
