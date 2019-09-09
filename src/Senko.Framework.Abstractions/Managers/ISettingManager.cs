using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Senko.Framework.Managers
{
    public interface ISettingManager
    {
        /// <summary>
        /// Get the value from the database.
        /// </summary>
        /// <param name="guildId">The guild ID.</param>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The value of the setting.</returns>
        Task<T> GetAsync<T>(ulong guildId, string key);

        /// <summary>
        /// Get the setting value from the database.
        /// </summary>
        /// <param name="guildId">The guild ID.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        Task SetAsync<T>(ulong guildId, string key, T value);
    }
}
