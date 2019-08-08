using System;
using System.Threading;
using System.Threading.Tasks;

namespace Senko.Framework.Hosting
{
    public interface IBotHost : IDisposable
    {
        /// <summary>
        ///     Starts listening to the Discord WebSocket server.
        /// </summary>
        void Start();

        /// <summary>
        ///     Starts listening to the Discord WebSocket server.
        /// </summary>
        /// <param name="token"></param>
        Task StartAsync(CancellationToken token = default);

        /// <summary>
        ///     Attempt to gracefully stop the host.
        /// </summary>
        /// <param name="token"></param>
        Task StopAsync(CancellationToken token = default);
    }
}
