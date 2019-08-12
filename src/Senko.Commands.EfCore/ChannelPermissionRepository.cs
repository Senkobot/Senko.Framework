using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;

namespace Senko.Commands.EfCore
{
    public class ChannelPermissionRepository<TContext> : EfCoreRepository<TContext, ChannelPermission>, IChannelPermissionRepository
        where TContext : DbContext
    {
        public ChannelPermissionRepository(TContext context)
            : base(context)
        {
        }

        public IQueryable<ChannelPermission> Query(ulong guildId, ulong channelId)
        {
            return Set.Where(rp => rp.GuildId == guildId && rp.ChannelId == channelId);
        }

        public Task<ChannelPermission> GetAsync(ulong guildId, ulong channelId, string permission)
        {
            return Query(guildId, channelId).FirstOrDefaultAsync(rp => rp.Name == permission);
        }
    }
}
