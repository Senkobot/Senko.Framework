using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundatio.Caching;
using Senko.Common;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Events;

namespace Senko.Commands.EventHandlers
{
    public class PermissionEventListener : IEventListener
    {
        private readonly ICacheClient _cache;

        public PermissionEventListener(ICacheClient cache)
        {
            _cache = cache;
        }

        [EventListener(EventPriority.High)]
        public Task OnMemberRolesUpdatedEvent(GuildMemberRolesUpdateEvent e)
        {
            return _cache.RemoveAsync(CacheKey.GetUserPermissionCacheKey(e.Member.GuildId, e.Member.Id));
        }
    }
}
