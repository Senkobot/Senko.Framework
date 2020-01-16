using System;
using System.Collections.Generic;
using System.Text;

namespace Senko.Commands
{
    public class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string name = null)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
