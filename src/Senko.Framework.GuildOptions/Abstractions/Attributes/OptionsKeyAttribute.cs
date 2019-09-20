using System;

namespace Senko.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OptionsKeyAttribute : Attribute
    {
        public OptionsKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}
