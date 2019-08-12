using Senko.Framework;
using Xunit;

namespace Senko.Commands.Tests.Modules
{
    public class FooModule : IModule
    {
        [Command("test", PermissionGroup.Moderator)]
        public void TestCommand(MessageContext context, string arg)
        {
            Assert.Equal("Foo", arg);
            context.Items["success"] = true;
        }
    }

    [CoreModule]
    public class CoreModule : IModule
    {
    }

    [DefaultModule]
    public class DefaultModule : IModule
    {
    }
}