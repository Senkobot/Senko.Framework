using System.Collections.Concurrent;
using System.Threading.Tasks;
using Senko.Framework.Repositories;

namespace Senko.Framework.Repository
{
    public class MemoryGuildOptionRepository : IGuildOptionRepository
    {
        private readonly ConcurrentDictionary<(ulong guildId, string key), string> _values;

        public MemoryGuildOptionRepository()
        {
            _values = new ConcurrentDictionary<(ulong guildId, string key), string>();
        }

        public Task<string> GetAsync(ulong guildId, string key)
        {
            return Task.FromResult(_values.TryGetValue((guildId, key), out var value) ? value : null);
        }

        public Task SetAsync(ulong guildId, string key, string value)
        {
            _values[(guildId, key)] = value;
            return Task.CompletedTask;
        }
    }
}
