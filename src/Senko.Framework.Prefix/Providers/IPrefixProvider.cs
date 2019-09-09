using System.Collections.Generic;
using System.Threading.Tasks;

namespace Senko.Framework.Prefix.Providers
{
    public interface IPrefixProvider
    {
        /// <summary>
        ///     Get the prefixes for the given context.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The prefixes.</returns>
        Task<IEnumerable<string>> GetPrefixesAsync(MessageContext context);
    }
}
