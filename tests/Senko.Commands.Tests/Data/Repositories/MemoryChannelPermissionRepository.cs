using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;
using Senko.TestFramework;
using Senko.TestFramework.Repository;

namespace Senko.Commands.Tests.Data.Repositories
{
    public class MemoryChannelPermissionRepository : MemoryRepository<ChannelPermission>, IChannelPermissionRepository
    {
        public IQueryable<ChannelPermission> Query(ulong guildId, ulong channelId)
        {
            return Items
                .Where(rp => rp.GuildId == guildId && rp.ChannelId == channelId)
                .AsAsyncQueryable();
        }

        public Task<ChannelPermission> GetAsync(ulong guildId, ulong channelId, string permission)
        {
            return Query(guildId, channelId).FirstOrDefaultAsync(rp => rp.Name == permission);
        }
    }
}
