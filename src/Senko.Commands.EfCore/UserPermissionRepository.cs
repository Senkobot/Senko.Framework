using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;

namespace Senko.Commands.EfCore
{
    public class UserPermissionRepository<TContext> : EfCoreRepository<TContext, UserPermission>, IUserPermissionRepository
        where TContext : DbContext
    {
        public UserPermissionRepository(TContext context)
            : base(context)
        {
        }

        public IQueryable<UserPermission> Query(ulong guildId, ulong userId)
        {
            return Set.Where(up => up.GuildId == guildId && up.UserId == userId);
        }

        public Task<UserPermission> GetAsync(ulong guildId, ulong userId, string permission)
        {
            return Query(guildId, userId).FirstOrDefaultAsync(up => up.Name == permission);
        }
    }
}
