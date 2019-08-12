using System.Collections.Generic;
using System.Threading.Tasks;

namespace Senko.Commands.Managers
{
    public interface IModuleManager
    {
        /// <summary>
        ///     All the module names.
        /// </summary>
        IReadOnlyList<string> ModuleNames { get; }

        /// <summary>
        ///     Get the enabled modules in the given <see cref="guildId"/>.
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<string>> GetEnabledModulesAsync(ulong guildId);

        /// <summary>
        ///     Sets if the module is enabled in the guild.
        /// </summary>
        /// <param name="guildId">The guild ID.</param>
        /// <param name="moduleName">The module name.</param>
        /// <param name="enabled">True if the module is enabled.</param>
        /// <exception cref="ModuleNotDisableException">Thrown when the module cannot be disabled or enabled.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the module name was not found in <see cref="ModuleNames"/>.</exception>
        Task SetModuleEnabledAsync(ulong guildId, string moduleName, bool enabled);
    }
}
