using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Senko.Framework.Hosting;

namespace Senko.TestFramework.Hosting
{
    internal class VoidBotApplication : IBotApplication
    {
        public Task StartAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
