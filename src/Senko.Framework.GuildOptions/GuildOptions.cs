using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Framework.Managers;

namespace Senko.Framework
{
    public struct GuildOptions<T>: IGuildOptions<T> where T : class, new()
    {
        private readonly IGuildOptionsManager _manager;
        private readonly ulong _guildId;
        private readonly string _key;

        public GuildOptions(IGuildOptionsManager manager, T value, ulong guildId, string key)
        {
            _manager = manager;
            Value = value;
            _guildId = guildId;
            _key = key;
        }

        public T Value { get; }

        public Task StoreAsync()
        {
            return _manager.SetAsync(_guildId, _key, Value);
        }
    }
}
