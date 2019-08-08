using System.Threading;
using System.Threading.Tasks;

namespace Senko.Framework.Hosting
{
    public interface IBotApplication
    {
        Task StartAsync(CancellationToken token);

        Task StopAsync(CancellationToken token);
    }
}