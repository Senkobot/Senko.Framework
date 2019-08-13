using System;

namespace Senko.Commands
{
    /// <summary>
    ///     Don't escape the pings (@everyone, @here, &lt;@00000000000000&gt;) when parsing the <see cref="string"/> argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class UnsafeAttribute : Attribute
    {
    }
}
