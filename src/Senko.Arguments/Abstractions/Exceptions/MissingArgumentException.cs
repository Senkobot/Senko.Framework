using System;

namespace Senko.Arguments.Abstractions.Exceptions
{
    public class MissingArgumentException : Exception
    {
        public MissingArgumentException(ArgumentType argumentType, string argumentName, string message) : base(message)
        {
            ArgumentType = argumentType;
            ArgumentName = argumentName;
        }

        public string ArgumentName { get; }

        public ArgumentType ArgumentType { get; }
    }
}
