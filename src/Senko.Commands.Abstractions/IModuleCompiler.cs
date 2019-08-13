using System.Collections.Generic;

namespace Senko.Commands
{
    public interface IModuleCompiler
    {
        /// <summary>
        ///     Extract the <see cref="ICommand"/> out of the <see cref="IModule"/>s.
        /// </summary>
        /// <param name="modules">The modules.</param>
        /// <returns>The commands.</returns>
        IEnumerable<ICommand> Compile(IEnumerable<IModule> modules);
    }
}
