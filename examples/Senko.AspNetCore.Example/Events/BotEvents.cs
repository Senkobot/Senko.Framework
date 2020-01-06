using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Senko.AspNetCore.Example.Hubs;
using Senko.Discord;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Events;

namespace Senko.AspNetCore.Example.Events
{
    public class BotEvents : IEventListener
    {
        private readonly IHubContext<BotHub> _hubContext;

        public BotEvents(IHubContext<BotHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [EventListener]
        public Task OnMessageReceived(MessageReceivedEvent e)
        {
            return _hubContext.Clients.All.SendAsync("ReceiveMessage", e.Author.GetDisplayName() + ": " + e.Content);
        }
    }
}
