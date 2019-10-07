using System.Collections.Generic;
using Senko.Common.Collections;

namespace Senko.Framework.Prefix.Providers
{
    public class ListPrefixProvider : IPrefixProvider
    {
        private readonly SyncAsyncEnumerable<string> _enumerable;

        public ListPrefixProvider(IEnumerable<string> prefixes)
        {
            _enumerable = new SyncAsyncEnumerable<string>(prefixes);
        }

        public IAsyncEnumerable<string> GetPrefixesAsync(MessageContext context)
        {
            return _enumerable;
        }
    }
}
