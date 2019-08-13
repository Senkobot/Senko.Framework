using System.Collections.Generic;
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

        public async Task<IReadOnlyList<GuildModule>> GetAllAsync(ulong guildId)
        {
            return await Set.Where(gm => gm.GuildId == guildId).ToArrayAsync();
        }

        public Task<GuildModule> GetAsync(ulong guildId, string moduleName)
        {
            return Set.FirstOrDefaultAsync(gm => gm.GuildId == guildId && gm.Name == moduleName);
        }
    }
}
