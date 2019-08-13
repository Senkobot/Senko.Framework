using System;

namespace Senko.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionAttribute : Attribute
    {
        public PermissionAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
