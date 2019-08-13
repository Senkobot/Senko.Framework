using System;
using Senko.Common;

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
