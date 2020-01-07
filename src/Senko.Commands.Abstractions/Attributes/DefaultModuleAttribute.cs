using System;

namespace Senko.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultModuleAttribute : ModuleAttribute
    {
        public DefaultModuleAttribute(string name = null) : base(name)
        {
        }
    }
}
