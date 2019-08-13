using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Senko.Commands.Managers;
using Senko.Discord.Events;
using Senko.Events;
using Senko.Framework;

namespace Senko.Commands.Events
{
    public class ModuleEventManager : EventManager
    {
        private readonly IModuleManager _moduleManager;

        public ModuleEventManager(
            IServiceProvider serviceProvider,
            IMessageContextAccessor contextAccessor,
            IModuleManager moduleManager
        ) : base(serviceProvider, contextAccessor)
        {
            _moduleManager = moduleManager;
        }

        protected override Task<IReadOnlyCollection<string>> GetEnabledModulesAsync(IEvent @event)
        {
            return @event is IGuildEvent guildEvent && guildEvent.GuildId.HasValue
                ? _moduleManager.GetEnabledModulesAsync(guildEvent.GuildId.Value)
                : base.GetEnabledModulesAsync(@event);
        }
    }
}
