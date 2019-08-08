using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;
using Senko.TestFramework;
using Senko.TestFramework.Repository;

namespace Senko.Modules.Tests.Data.Repositories
{
    public class MemoryUserPermissionRepository : MemoryRepository<UserPermission>, IUserPermissionRepository
    {
        public IQueryable<UserPermission> Query(ulong guildId, ulong userId)
        {
            return Items
                .Where(up => up.GuildId == guildId && up.UserId == userId)
                .AsAsyncQueryable();
        }

        public Task<UserPermission> GetAsync(ulong guildId, ulong userId, string permission)
        {
            return Query(guildId, userId).FirstOrDefaultAsync(up => up.Name == permission);
        }
    }
}
