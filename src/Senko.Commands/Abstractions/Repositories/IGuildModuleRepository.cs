using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Senko.Commands.Entities;

namespace Senko.Commands.Repositories
{
    public interface IGuildModuleRepository
    {
        Task<GuildModule> GetAsync(ulong guildId, string moduleName);

        IQueryable<GuildModule> Query(ulong guildId);

        void Remove(GuildModule entity);

        void Add(GuildModule entity);

        void Update(GuildModule entity);

        Task SaveChangesAsync(CancellationToken token = default);
    }
}
