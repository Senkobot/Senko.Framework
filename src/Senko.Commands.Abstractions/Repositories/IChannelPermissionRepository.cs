using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Senko.Commands.Entities;

namespace Senko.Commands.Repositories
{
    public interface IChannelPermissionRepository
    {
        Task<IReadOnlyList<ChannelPermission>> GetAllAsync(ulong guildId, ulong channelId);
        
        Task<ChannelPermission> GetAsync(ulong guildId, ulong channelId, string permission);

        void Add(ChannelPermission entity);

        void Update(ChannelPermission entity);

        void Remove(ChannelPermission entity);

        Task SaveChangesAsync(CancellationToken token = default);
    }
}
