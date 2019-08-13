using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Senko.Commands.Entities;

namespace Senko.Commands.Repositories
{
    public interface IGuildModuleRepository
    {
        Task<IReadOnlyList<GuildModule>> GetAllAsync(ulong guildId);

        Task<GuildModule> GetAsync(ulong guildId, string moduleName);

        void Remove(GuildModule entity);

        void Add(GuildModule entity);

        void Update(GuildModule entity);

        Task SaveChangesAsync(CancellationToken token = default);
    }
}
