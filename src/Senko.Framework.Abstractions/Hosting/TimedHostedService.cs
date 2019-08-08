using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Senko.Framework.Hosting;

namespace Senko.Common.Services
{
    public abstract class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        protected TimedHostedService(ILogger logger)
        {
            _logger = logger;
        }

        public abstract TimeSpan Period { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Timed Background Service {GetType().GetFriendlyName()} is starting.");

            if (_timer == null) 
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, Period);
            }
            else
            {
                _timer?.Change(TimeSpan.Zero, Period);
            }

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Task.Run(DoWorkAsync);
        }

        public abstract Task DoWorkAsync();

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Timed Background Service {GetType().GetFriendlyName()} is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
