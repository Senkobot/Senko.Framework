using System.Threading.Tasks;

namespace Senko.Framework.Repositories
{
    public interface IGuildOptionRepository
    {
        /// <summary>
        /// Get the value from the database.
        /// </summary>
        /// <param name="guildId">The guild ID.</param>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The value of the setting.</returns>
        Task<string> GetAsync(ulong guildId, string key);

        /// <summary>
        /// Get the setting value from the database.
        /// </summary>
        /// <param name="guildId">The guild ID.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        Task SetAsync(ulong guildId, string key, string value);
    }
}
