using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Arguments;
using Senko.Commands.Roslyn;
using Senko.Framework;
using Senko.Modules.Tests.Modules;
using Senko.TestFramework;
using Xunit;

namespace Senko.Modules.Tests.Roslyn
{
    public class RoslynCommandBuilderTest
    {
        private MessageContext CreateContext(string message)
        {
            var services = new EventServiceCollection();

            services.AddTestClient();
            services.AddArgumentWithParsers();

            return new DefaultMessageContext
            {
                RequestServices = services.BuildServiceProvider(),
                Request =
                {
                    Message = message
                }
            };
        }

        [Fact]
        public async Task TestInvokeCommand()
        {
            var compiler = new RoslynCommandBuilder();
            var module = new FooModule();

            compiler.AddModule(module);

            var commands = compiler.Compile().ToDictionary(c => c.Id, c => c);

            Assert.True(commands.TryGetValue("test", out var testCommand));
            Assert.Equal("test", testCommand.Id);

            var context = CreateContext("Foo");

            await testCommand.ExecuteAsync(context);

            Assert.Equal(true, context.Items["success"]);
        }
    }
}
