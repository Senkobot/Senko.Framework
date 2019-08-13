using System.Collections.Generic;
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

        public async Task<IReadOnlyList<UserPermission>> GetAllAsync(ulong guildId, ulong userId)
        {
            return await Set.Where(up => up.GuildId == guildId && up.UserId == userId).ToArrayAsync();
        }

        public Task<UserPermission> GetAsync(ulong guildId, ulong userId, string permission)
        {
            return Set.FirstOrDefaultAsync(up => up.GuildId == guildId && up.UserId == userId && up.Name == permission);
        }
    }
}
