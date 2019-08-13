using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;

namespace Senko.Commands.EfCore
{
    public class RolePermissionRepository<TContext> : EfCoreRepository<TContext, RolePermission>, IRolePermissionRepository
        where TContext : DbContext
    {
        public RolePermissionRepository(TContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyList<RolePermission>> GetAllAsync(ulong guildId, ulong roleId)
        {
            return await Set.Where(rp => rp.GuildId == guildId && rp.RoleId == roleId).ToArrayAsync();
        }

        public Task<RolePermission> GetAsync(ulong guildId, ulong roleId, string permission)
        {
            return Set.FirstOrDefaultAsync(rp => rp.GuildId == guildId && rp.RoleId == roleId && rp.Name == permission);
        }
    }
}
