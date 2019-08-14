using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Senko.Framework.Repositories
{
    internal class DefaultSettingRepository : ISettingRepository
    {
        public Task<string> GetAsync(ulong guildId, string key)
        {
            return Task.FromResult(string.Empty);
        }

        public Task SetAsync(ulong guildId, string key, string value)
        {
            throw new NotSupportedException("The setting repository is not provided. Please register the ISettingRepository.");
        }
    }
}
