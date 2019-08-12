using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;
using Senko.TestFramework;
using Senko.TestFramework.Repository;

namespace Senko.Commands.Tests.Data.Repositories
{
    public class MemoryRolePermissionRepository : MemoryRepository<RolePermission>, IRolePermissionRepository
    {
        public IQueryable<RolePermission> Query(ulong guildId, ulong roleId)
        {
            return Items
                .Where(rp => rp.GuildId == guildId && rp.RoleId == roleId)
                .AsAsyncQueryable();
        }

        public Task<RolePermission> GetAsync(ulong guildId, ulong roleId, string permission)
        {
            return Query(guildId, roleId).FirstOrDefaultAsync(rp => rp.Name == permission);
        }
    }
}
