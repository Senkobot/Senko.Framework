using System.Threading.Tasks;
using Senko.Framework;

namespace Senko.Commands
{
    public interface ICommand
    {
        /// <summary>
        ///     The unique identifier of the command.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The module where the command belongs to.
        /// </summary>
        string Module { get; }

        /// <summary>
        ///     The permission that is required to execute the command.
        /// </summary>
        string Permission { get; }

        /// <summary>
        ///     The default permission group of the command.
        /// </summary>
        PermissionGroup PermissionGroup { get; }

        /// <summary>
        ///     True if the command can only be executed in guild channels.
        /// </summary>
        bool GuildOnly { get; }

        /// <summary>
        ///     Execute the command.
        /// </summary>
        /// <param name="context">The <see cref="MessageContext"/>.</param>
        Task ExecuteAsync(MessageContext context);
    }
}
