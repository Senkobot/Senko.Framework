using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Arguments;
using Senko.Commands.Managers;
using Senko.Commands.Reflection;
using Senko.Commands.Roslyn;
using Senko.Framework;
using Senko.Localization;
using Senko.TestFramework;
using Xunit;

namespace Senko.Commands.Tests.Modules
{
    public class ModuleBuilderTest
    {
        private static MessageContext CreateContext(Type type, string message)
        {
            var services = new ServiceCollection();

            services.AddSingleton(typeof(IModuleCompiler), type);
            services.AddCommand()
                .AddModule<FooModule>();
            services.AddLocalizations();
            services.AddArgumentWithParsers();

            return new DefaultMessageContext
            {
                RequestServices = services.BuildTestServiceProvider(),
                Request =
                {
                    Message = message
                }
            };
        }

        [Theory]
        [InlineData(typeof(RoslynModuleCompiler))]
        [InlineData(typeof(ReflectionModuleCompiler))]
        public async Task TestBuilder(Type type)
        {
            var context = CreateContext(type, "Foo");
            var commandManager = context.RequestServices.GetRequiredService<ICommandManager>();
            var command = commandManager.Commands.FirstOrDefault(c => c.Id == "test");
            var aliasCommand = commandManager.FindAll("test_alias").FirstOrDefault();

            Assert.NotNull(command);
            Assert.Equal(command, aliasCommand);

            await command.ExecuteAsync(context);

            Assert.Equal(true, context.Items["success"]);
        }
    }
}
