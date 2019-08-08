using System.Threading;
using System.Threading.Tasks;

namespace Senko.Framework.Hosting
{
    public interface IHostedService
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}
