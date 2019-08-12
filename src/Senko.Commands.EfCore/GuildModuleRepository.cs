using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;

namespace Senko.Commands.EfCore
{
    public class GuildModuleRepository<TContext> : EfCoreRepository<TContext, GuildModule>, IGuildModuleRepository
        where TContext : DbContext
    {
        public GuildModuleRepository(TContext context) : base(context)
        {
        }

        public Task<GuildModule> GetAsync(ulong guildId, string moduleName)
        {
            return Set.FirstOrDefaultAsync(gm => gm.GuildId == guildId && gm.Name == moduleName);
        }

        public IQueryable<GuildModule> Query(ulong guildId)
        {
            return Set.Where(gm => gm.GuildId == guildId);
        }
    }
}
