using System.Threading.Tasks;
using Senko.Commands.Example.Options;
using Senko.Discord;
using Senko.Discord.Packets;
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
        [Alias("ok")]
        public void React(MessageContext context)
        {
            context.Response.React(Emoji.WhiteCheckMark);
        }

        [Command("set")]
        public Task SetAsync(
            MessageContext context,
            IGuildOptions<ExampleOptions> options,
            [Remaining] string value)
        {
            options.Value.Content = value;
            context.Response.React(Emoji.OkHand);

            return options.StoreAsync();
        }

        [Command("get")]
        public void Get(
            MessageContext context,
            IGuildOptions<ExampleOptions> options)
        {
            context.Response.AddMessage(options.Value.Content ?? "No content saved");
        }
    }
}
