using System.Collections.Generic;
using System.Globalization;

namespace Senko.Commands.Managers
{
    public interface ICommandManager
    {
        /// <summary>
        ///     Get all the commands that are registered.
        /// </summary>
        IReadOnlyList<ICommand> Commands { get; }

        /// <summary>
        ///     Find the command by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="culture">The culture to use. Null if the current culture should be used.</param>
        /// <returns>The command.</returns>
        IReadOnlyCollection<ICommand> FindAll(string name, CultureInfo culture = null);

        /// <summary>
        ///     Get the name of the command in the given <see cref="culture"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        string GetName(string id, CultureInfo culture = null);
    }
}
