using Senko.Framework;
using Xunit;

namespace Senko.Commands.Tests.Modules
{
    public class FooModule
    {
        [Command("foo", PermissionGroup.Moderator)]
        [Alias("foo_alias")]
        public void FooCommand(MessageContext context, string arg)
        {
            Assert.Equal("Foo", arg);
            context.Items["success"] = true;
        }
        
        [Command("bar", PermissionGroup.Moderator)]
        [Alias("bar_alias")]
        public void BarCommand(MessageContext context, string arg)
        {
            Assert.Equal("Bar", arg);
            context.Items["success"] = true;
        }

        [Command("int")]
        public void IntCommand(MessageContext context, int value)
        {
            Assert.Equal(0, value);
            context.Items["success"] = true;
        }
    }

    [CoreModule]
    public class CoreModule
    {
    }

    [DefaultModule]
    public class DefaultModule
    {
    }
}