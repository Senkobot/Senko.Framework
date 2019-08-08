﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Senko.Commands.Entities;

namespace Senko.Commands.Repositories
{
    public interface IRolePermissionRepository
    {
        IQueryable<RolePermission> Query(ulong guildId, ulong roleId);

        Task<RolePermission> GetAsync(ulong guildId, ulong roleId, string permission);

        void Add(RolePermission entity);

        void Update(RolePermission entity);

        void Remove(RolePermission entity);

        Task SaveChangesAsync(CancellationToken token = default);
    }
}
