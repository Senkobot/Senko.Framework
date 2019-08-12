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

        public IQueryable<RolePermission> Query(ulong guildId, ulong roleId)
        {
            return Set.Where(rp => rp.GuildId == guildId && rp.RoleId == roleId);
        }

        public Task<RolePermission> GetAsync(ulong guildId, ulong roleId, string permission)
        {
            return Query(guildId, roleId).FirstOrDefaultAsync(rp => rp.Name == permission);
        }
    }
}
