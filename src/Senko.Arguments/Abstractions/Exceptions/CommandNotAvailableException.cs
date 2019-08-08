using System;

namespace Senko.Arguments.Abstractions.Exceptions
{
    public class CommandNotAvailableException : Exception
    {
        public CommandNotAvailableException(string argumentName, string message) : base(message)
        {
            ArgumentName = argumentName;
        }

        public string ArgumentName { get; }
    }
}
