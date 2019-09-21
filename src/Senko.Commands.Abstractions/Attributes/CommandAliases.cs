using System;
using System.Collections.Generic;
using System.Text;

namespace Senko.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Alias : Attribute
    {
        public Alias(params string[] aliases)
        {
            Aliases = aliases;
        }

        public IReadOnlyList<string> Aliases { get; }
    }
}
