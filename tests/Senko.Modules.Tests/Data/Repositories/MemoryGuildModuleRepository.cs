using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;
using Senko.TestFramework;
using Senko.TestFramework.Repository;

namespace Senko.Modules.Tests.Data.Repositories
{
    public class MemoryGuildModuleRepository : MemoryRepository<GuildModule>, IGuildModuleRepository
    {
        public Task<GuildModule> GetAsync(ulong guildId, string moduleName)
        {
            return Query(guildId).FirstOrDefaultAsync(gm => gm.Name == moduleName);
        }

        public IQueryable<GuildModule> Query(ulong guildId)
        {
            return Items
                .Where(gm => gm.GuildId == guildId)
                .AsAsyncQueryable();
        }
    }
}
