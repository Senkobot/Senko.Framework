using System;

namespace Senko.Arguments.Exceptions
{
    public class MissingArgumentException : Exception
    {
        public MissingArgumentException(Type argumentType, string argumentName, string message) : base(message)
        {
            ArgumentType = argumentType;
            ArgumentName = argumentName;
        }

        public string ArgumentName { get; }

        public Type ArgumentType { get; }
    }
}
