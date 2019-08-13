using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;
using Senko.Common;

namespace Senko.Commands.EfCore
{
    public class ChannelPermissionRepository<TContext> : EfCoreRepository<TContext, ChannelPermission>, IChannelPermissionRepository
        where TContext : DbContext
    {
        public ChannelPermissionRepository(TContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyList<ChannelPermission>> GetAllAsync(ulong guildId, ulong channelId)
        {
            return await Set.Where(rp => rp.GuildId == guildId && rp.ChannelId == channelId).ToArrayAsync();
        }

        public Task<ChannelPermission> GetAsync(ulong guildId, ulong channelId, string permission)
        {
            return Set.FirstOrDefaultAsync(rp => rp.GuildId == guildId && rp.ChannelId == channelId && rp.Name == permission);
        }
    }
}
