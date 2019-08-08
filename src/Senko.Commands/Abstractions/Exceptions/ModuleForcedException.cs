using System;

namespace Senko.Commands
{
    public class ModuleForcedException : Exception
    {
        public ModuleForcedException(string moduleName)
            : base($"The module {moduleName} cannot be disabled")
        {
            ModuleName = moduleName;
        }

        public string ModuleName { get; }
    }
}
