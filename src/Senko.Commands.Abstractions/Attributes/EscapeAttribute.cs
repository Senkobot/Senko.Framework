using System;
using Senko.Discord;

namespace Senko.Commands
{
    public class EscapeAttribute : Attribute
    {
        public EscapeAttribute(EscapeType type)
        {
            Type = type;
        }

        public EscapeType Type { get; }
    }
}
