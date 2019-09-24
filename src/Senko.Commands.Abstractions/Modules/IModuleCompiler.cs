using System;
using System.Collections.Generic;

namespace Senko.Commands
{
    public interface IModuleCompiler
    {
        /// <summary>
        ///     Extract the <see cref="ICommand"/> out of the module types.
        /// </summary>
        /// <param name="typesEnumerable">The typesEnumerable.</param>
        /// <returns>The commands.</returns>
        IEnumerable<ICommand> Compile(IEnumerable<Type> typesEnumerable);
    }
}
