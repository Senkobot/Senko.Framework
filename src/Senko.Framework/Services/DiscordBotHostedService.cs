using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Senko.Events;
using Senko.Framework.Events;
using Senko.Framework.Hosting;

namespace Senko.Framework.Services
{
    internal class DiscordBotHostedService : IHostedService
    {
        private readonly IBotApplication _botApplication;
        private readonly IEventManager _eventManager;

        public DiscordBotHostedService(IBotApplication botApplication, IEventManager eventManager)
        {
            _botApplication = botApplication;
            _eventManager = eventManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _eventManager.CallAsync(new InitializeEvent());
            await _botApplication.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _botApplication.StopAsync(cancellationToken).AsTask();
        }
    }
}
