using System.Threading;
using System.Threading.Tasks;
using Senko.Framework.Hosting;

namespace Senko.Localization
{
    public class StringLocalizerHostedService : IHostedService
    {
        private readonly IStringLocalizer _localizer;

        public StringLocalizerHostedService(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _localizer.LoadAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
