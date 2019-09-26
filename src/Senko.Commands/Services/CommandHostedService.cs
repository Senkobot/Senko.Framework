using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Senko.Commands.Managers;
using Senko.Framework.Hosting;

namespace Senko.Commands.Services
{
    public class CommandHostedService : IHostedService
    {
        private readonly ICommandManager _commandManager;

        public CommandHostedService(ICommandManager commandManager)
        {
            _commandManager = commandManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _commandManager.Initialize();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
