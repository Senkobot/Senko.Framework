using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Senko.Framework.Hosting;

namespace Senko.Framework.Services
{
    public class FuncService : IHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly Func<IServiceProvider, Task> _func;

        public FuncService(Func<IServiceProvider, Task> func, IServiceProvider provider)
        {
            _func = func;
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();

            await _func(scope.ServiceProvider);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
