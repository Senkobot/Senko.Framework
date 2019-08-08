using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Senko.Discord;

namespace Senko.Framework
{
    public abstract class MessageContext
    {
        public abstract IServiceProvider RequestServices { get; set; }

        public abstract IDiscordUser User { get; set; }

        public abstract IDiscordUser Self { get; set; }

        public abstract IFeatureCollection Features { get; }

        public abstract MessageRequest Request { get; }

        public abstract MessageResponse Response { get; }

        public abstract IDictionary<object, object> Items { get; set; }
    }
}
