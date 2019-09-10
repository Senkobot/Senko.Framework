using System.Collections.Generic;
using System.Threading.Tasks;

namespace Senko.Framework.Prefix.Providers
{
    public class ListPrefixProvider : IPrefixProvider
    {
        private readonly IReadOnlyCollection<string> _prefixes;

        public ListPrefixProvider(IReadOnlyCollection<string> prefixes)
        {
            _prefixes = prefixes;
        }

        public Task<IEnumerable<string>> GetPrefixesAsync(MessageContext context)
        {
            return Task.FromResult<IEnumerable<string>>(_prefixes);
        }
    }
}
