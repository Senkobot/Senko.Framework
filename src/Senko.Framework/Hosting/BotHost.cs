using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Senko.Events;
using Senko.Framework.Events;

namespace Senko.Framework.Hosting
{
    public class BotHost : IBotHost
    {
        private readonly IServiceCollection _applicationServices;

        public BotHost(IServiceCollection applicationServices)
        {
            _applicationServices = applicationServices;
        }

        public ServiceProvider CurrentProvider { get; private set; }

        public void Start()
        {
            StartAsync().Wait();
        }

        public async Task StartAsync(CancellationToken token = default)
        {
            CurrentProvider = _applicationServices.BuildServiceProvider();

            var eventManager = CurrentProvider.GetRequiredService<IEventManager>();
            await eventManager.CallAsync(new InitializeEvent());

            foreach (var hostedService in CurrentProvider.GetServices<IHostedService>())
            {
                await hostedService.StartAsync(token);
            }

            var application = CurrentProvider.GetRequiredService<IBotApplication>();
            await application.StartAsync(token);
        }

        public async Task StopAsync(CancellationToken token = default)
        {
            var application = CurrentProvider.GetRequiredService<IBotApplication>();

            foreach (var hostedService in CurrentProvider.GetServices<IHostedService>())
            {
                await hostedService.StopAsync(token);
            }

            await application.StopAsync(token);

            CurrentProvider.Dispose();
            CurrentProvider = null;
        }

        public void Dispose()
        {
            CurrentProvider.Dispose();
        }
    }
}
