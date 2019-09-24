using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Arguments;
using Senko.Commands.Managers;
using Senko.Framework;
using Senko.Localization;
using Senko.TestFramework;
using Xunit;

namespace Senko.Commands.Tests.Modules
{
    public class ExecuteTest
    {
        private async Task<MessageContext> ExecuteCommandAsync(string commandName, string message)
        {
            var services = new EventServiceCollection();

            services.AddCommand()
                .AddModule<FooModule>();

            services.AddLocalizations();
            services.AddArgumentWithParsers();

            var provider = services.BuildTestServiceProvider();
            var context = new DefaultMessageContext
            {
                RequestServices = provider,
                Request =
                {
                    Message = message
                }
            };
            
            var commandManager = context.RequestServices.GetRequiredService<ICommandManager>();
            var command = commandManager.Commands.FirstOrDefault(c => c.Id == commandName);
            
            Assert.NotNull(command);
            
            await command.ExecuteAsync(context);

            return context;
        }

        [Fact]
        public async Task TestIntAsync()
        {
            var context = await ExecuteCommandAsync("int", "0");

            Assert.Equal(true, context.Items["success"]);
        }
    }
}
