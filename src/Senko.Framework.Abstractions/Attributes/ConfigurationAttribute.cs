using System;

namespace Senko.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationAttribute : Attribute
    {
        public ConfigurationAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
