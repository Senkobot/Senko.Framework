using System.Collections.Generic;
using System.Threading;

namespace Senko.Common.Collections
{
    public class SyncAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        public SyncAsyncEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new SyncAsyncEnumerator<T>(_enumerable.GetEnumerator());
        }
    }
}