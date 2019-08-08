using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Events;
using Senko.Framework.Events;

namespace Senko.Framework.Hosting
{
    public class BotHost : IBotHost
    {
        private readonly IServiceCollection _applicationServices;
        private ServiceProvider _currentProvider;

        public BotHost(IServiceCollection applicationServices)
        {
            _applicationServices = applicationServices;
        }
        
        public void Start()
        {
            StartAsync().Wait();
        }

        public async Task StartAsync(CancellationToken token = default)
        {
            _currentProvider = _applicationServices.BuildServiceProvider();

            var eventManager = _currentProvider.GetRequiredService<IEventManager>();
            await eventManager.CallAsync(new InitializeEvent());

            var application = _currentProvider.GetRequiredService<IBotApplication>();
            await application.StartAsync(token);

            foreach (var hostedService in _currentProvider.GetServices<IHostedService>())
            {
                await hostedService.StartAsync(token);
            }
        }

        public async Task StopAsync(CancellationToken token = default)
        {
            var application = _currentProvider.GetRequiredService<IBotApplication>();

            foreach (var hostedService in _currentProvider.GetServices<IHostedService>())
            {
                await hostedService.StopAsync(token);
            }

            await application.StopAsync(token);

            _currentProvider.Dispose();
            _currentProvider = null;
        }

        public void Dispose()
        {
            _currentProvider.Dispose();
        }
    }
}
