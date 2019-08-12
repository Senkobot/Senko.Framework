using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Discord;

namespace Senko.TestFramework.Services
{
    public class VoidDiscordEventHandler : IDiscordEventHandler
    {
        public Task OnGuildJoin(IDiscordGuild guild)
        {
            return Task.CompletedTask;
        }

        public Task OnUserUpdate(IDiscordUser user)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildUnavailable(ulong guildId)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildLeave(ulong guildId)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberDelete(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberCreate(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageCreate(IDiscordMessage message)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageUpdate(IDiscordMessage message)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberRolesUpdate(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }
    }
}
