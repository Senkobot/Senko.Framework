using System.Threading;
using System.Threading.Tasks;
using Senko.Framework.Hosting;

namespace Senko.TestFramework.Hosting
{
    internal class VoidBotApplication : IBotApplication
    {
        public ValueTask StartAsync(CancellationToken token)
        {
            return default;
        }

        public ValueTask StopAsync(CancellationToken token)
        {
            return default;
        }
    }
}
