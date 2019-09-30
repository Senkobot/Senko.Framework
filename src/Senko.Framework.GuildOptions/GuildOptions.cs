using System.Threading.Tasks;
using Senko.Framework.Managers;

namespace Senko.Framework
{
    public struct GuildOptions<T>: IGuildOptions<T> where T : class, new()
    {
        private readonly IGuildOptionsManager _manager;
        private readonly ulong _guildId;

        public GuildOptions(IGuildOptionsManager manager, ulong guildId, T value)
        {
            _manager = manager;
            _guildId = guildId;
            Value = value;
        }

        public T Value { get; }

        public ValueTask StoreAsync()
        {
            return _manager.SetAsync(_guildId, Value);
        }
    }
}
