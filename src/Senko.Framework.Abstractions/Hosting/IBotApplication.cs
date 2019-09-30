using System.Threading;
using System.Threading.Tasks;

namespace Senko.Framework.Hosting
{
    public interface IBotApplication
    {
        ValueTask StartAsync(CancellationToken token);

        ValueTask StopAsync(CancellationToken token);
    }
}