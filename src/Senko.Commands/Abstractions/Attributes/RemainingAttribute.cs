using System;
using Senko.Arguments;

namespace Senko.Commands
{
    /// <summary>
    ///     Get the remaining text in the <see cref="IArgumentReader"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RemainingAttribute : Attribute
    {
    }
}
