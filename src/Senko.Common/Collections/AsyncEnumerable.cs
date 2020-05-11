using System.Collections.Generic;
using System.Linq;
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

    public static class AsyncEnumerable
    {
        public static IAsyncEnumerable<T> From<T>(IEnumerable<T> enumerable)
        {
            return new SyncAsyncEnumerable<T>(enumerable);
        }

        public static IAsyncEnumerable<T> Empty<T>()
        {
            return new SyncAsyncEnumerable<T>(Enumerable.Empty<T>());
        }
    }
}