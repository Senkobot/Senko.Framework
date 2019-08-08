using Senko.Commands;
using Senko.Framework;
using Xunit;

namespace Senko.Modules.Tests.Modules
{
    public class FooModule : IModule
    {
        [Command("test")]
        public void TestCommand(MessageContext context, string arg)
        {
            Assert.Equal("Foo", arg);
            context.Items["success"] = true;
        }
    }
}