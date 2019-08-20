﻿using Senko.Discord;
using Senko.Framework;

namespace Senko.Commands.Example
{
    [CoreModule]
    public class ExampleModule : IModule
    {
        [Command("ping")]
        public void Ping(MessageContext context)
        {
            context.Response.AddMessage("Pong");
        }
        
        [Command("greet")]
        public void Greet(MessageContext context, IDiscordUser user)
        {
            context.Response.AddMessage("Hello " + user.GetDisplayName());
        }

        [Command("react")]
        public void React(MessageContext context)
        {
            context.Response.React(Emoji.WhiteCheckMark);
        }
    }
}
