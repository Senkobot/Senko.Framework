using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Discord;
using Senko.Discord.Packets;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Discord;
using Senko.Framework.Events;
using Senko.Framework.Features;
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
            return _client.Gateway.StartAsync();
        }

        public ValueTask StopAsync(CancellationToken token)
        {
            return _client.Gateway.StopAsync();
        }
    }
}
