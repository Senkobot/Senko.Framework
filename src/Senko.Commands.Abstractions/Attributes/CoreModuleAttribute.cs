using System;

namespace Senko.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CoreModuleAttribute : ModuleAttribute
    {
        public CoreModuleAttribute(string name = null) : base(name)
        {
        }
    }
}
