using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Discord;
using Senko.Events;
using Senko.Framework.Events;

namespace Senko.Framework
{
    public class DiscordEventHandler : IDiscordEventHandler
    {
        private readonly IEventManager _eventManager;
        private readonly IServiceProvider _provider;
        private IDiscordClient _client;

        public DiscordEventHandler(IEventManager eventManager, IServiceProvider provider)
        {
            _eventManager = eventManager;
            _provider = provider;
        }

        private IDiscordClient Client => _client ??= _provider.GetRequiredService<IDiscordClient>();

        public Task OnGuildJoin(IDiscordGuild guild)
        {
            return _eventManager.CallAsync(new GuildAvailableEvent(guild));
        }

        public Task OnGuildUpdate(IDiscordGuild guild)
        {
            return _eventManager.CallAsync(new GuildUpdateEvent(guild));
        }

        public Task OnUserUpdate(IDiscordUser user)
        {
            return _eventManager.CallAsync(new UserUpdateEvent(user));
        }

        public Task OnChannelCreate(IDiscordChannel channel)
        {
            return _eventManager.CallAsync(new ChannelCreateEvent(channel));
        }

        public Task OnChannelUpdate(IDiscordChannel channel)
        {
            return _eventManager.CallAsync(new ChannelUpdateEvent(channel));
        }

        public Task OnChannelDelete(IDiscordChannel channel)
        {
            return _eventManager.CallAsync(new ChannelDeleteEvent(channel));
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

        public Task OnGuildMemberUpdate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberUpdateEvent(member));
        }

        public Task OnGuildMemberCreate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberCreateEvent(member));
        }

        public Task OnGuildRoleCreate(ulong guildId, IDiscordRole role)
        {
            return _eventManager.CallAsync(new GuildRoleCreateEvent(guildId, role));
        }

        public Task OnGuildRoleUpdate(ulong guildId, IDiscordRole role)
        {
            return _eventManager.CallAsync(new GuildRoleUpdateEvent(guildId, role));
        }

        public Task OnGuildRoleDeleted(ulong guildId, IDiscordRole role)
        {
            return _eventManager.CallAsync(new GuildRoleDeleteEvent(guildId, role));
        }

        public Task OnMessageCreate(IDiscordMessage message)
        {
            return _eventManager.CallAsync(new MessageReceivedEvent(message));
        }

        public Task OnMessageUpdate(IDiscordMessage message)
        {
            return _eventManager.CallAsync(new MessageUpdateEvent(message));
        }

        public async Task OnMessageDeleted(ulong channelId, ulong messageId)
        {
            var channel = await Client.GetChannelAsync(channelId);

            await _eventManager.CallAsync(new MessageDeleteEvent(channelId, messageId, (channel as IDiscordGuildChannel)?.GuildId));
        }

        public Task OnGuildMemberRolesUpdate(IDiscordGuildUser member)
        {
            return _eventManager.CallAsync(new GuildMemberRolesUpdateEvent(member));
        }
    }
}
