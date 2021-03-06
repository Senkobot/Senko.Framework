﻿using System.Threading.Tasks;

namespace Senko.Framework.Managers
{
    public interface IGuildOptionsManager
    {
        /// <summary>
        /// Get the value from the database.
        /// </summary>
        /// <param name="guildId">The guild ID.</param>
        /// <returns>The value of the setting.</returns>
        ValueTask<T> GetAsync<T>(ulong guildId) where T : new();

        /// <summary>
        /// Get the setting value from the database.
        /// </summary>
        /// <param name="guildId">The guild ID.</param>
        /// <param name="value">The value of the setting.</param>
        ValueTask SetAsync<T>(ulong guildId, T value);
    }
}
