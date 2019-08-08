using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
