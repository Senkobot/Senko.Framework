using System.Threading.Tasks;
using Senko.Commands.Example.Options;
using Senko.Discord;
using Senko.Framework;

namespace Senko.Commands.Example
{
    [CoreModule]
    public class ExampleModule : ModuleBase
    {
        private readonly IGuildOptions<ExampleOptions> _options;

        public ExampleModule(IGuildOptions<ExampleOptions> options)
        {
            _options = options;
        }

        [Command("ping")]
        public void Ping()
        {
            Response.AddMessage("Pong");
        }
        
        [Command("greet")]
        public void Greet(IDiscordUser user)
        {
            Response.AddMessage("Hello " + user.GetDisplayName());
        }

        [Command("react")]
        [Alias("ok")]
        public void React()
        {
            Response.React(Emoji.WhiteCheckMark);
        }

        [Command("set")]
        public Task SetAsync([Remaining] string value)
        {
            _options.Value.Content = value;
            Response.React(Emoji.OkHand);

            return _options.StoreAsync();
        }

        [Command("get")]
        public void Get()
        {
            Response.AddMessage(_options.Value.Content ?? "No content saved");
        }
    }
}
