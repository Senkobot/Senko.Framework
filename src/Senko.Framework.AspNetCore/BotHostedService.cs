using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Senko.Discord;
using Senko.Events;
using Senko.Framework.Events;
using Senko.Framework.Hosting;

namespace Senko.Framework.AspNetCore
{
    public class BotHostedService : IHostedService
    {
        private readonly IBotApplication _botHost;
        private readonly IEventManager _eventManager;

        public BotHostedService(IBotApplication botHost, IEventManager eventManager)
        {
            _botHost = botHost;
            _eventManager = eventManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _eventManager.CallAsync(new InitializeEvent());
            await _botHost.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _botHost.StopAsync(cancellationToken).AsTask();
        }
    }
}
