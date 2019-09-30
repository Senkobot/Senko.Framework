using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Localization;

namespace Senko.Framework.Hosting
{
    [ExcludeFromCodeCoverage]
    public class BotApplication : IBotApplication
    {
        private readonly IDiscordClient _client;


        public BotApplication(IDiscordClient client, IStringLocalizer localizer)
        {
            _client = client;
        }

        public ValueTask StartAsync(CancellationToken token)
        {
            return _client.StartAsync();
        }

        public ValueTask StopAsync(CancellationToken token)
        {
            return _client.StopAsync();
        }
    }
}
