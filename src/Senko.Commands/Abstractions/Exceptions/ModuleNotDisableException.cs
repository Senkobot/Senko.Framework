using System;

namespace Senko.Commands
{
    public class ModuleNotDisableException : Exception
    {
        public ModuleNotDisableException(string moduleName, string message)
            : base(message)
        {
            ModuleName = moduleName;
        }

        public string ModuleName { get; }
    }
}
