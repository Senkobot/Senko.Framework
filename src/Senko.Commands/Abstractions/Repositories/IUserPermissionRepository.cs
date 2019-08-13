using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Senko.Commands.Entities;

namespace Senko.Commands.Repositories
{
    public interface IUserPermissionRepository
    {
        Task<IReadOnlyList<UserPermission>> GetAllAsync(ulong guildId, ulong userId);

        Task<UserPermission> GetAsync(ulong guildId, ulong userId, string permission);

        void Add(UserPermission entity);

        void Update(UserPermission entity);

        void Remove(UserPermission entity);

        Task SaveChangesAsync(CancellationToken token = default);
    }
}
