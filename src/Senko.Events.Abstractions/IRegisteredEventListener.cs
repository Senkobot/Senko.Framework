using System;

namespace Senko.Events
{
    public interface IRegisteredEventListener
    {
        /// <summary>
        ///     The type that the event is listening to.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     The priority of the event listener.
        /// </summary>
        EventPriority Priority { get; }
        
        /// <summary>
        ///     The order priority of the event listener.
        /// </summary>
        int PriorityOrder { get; set; }

        /// <summary>
        ///     If set to true, the listener will be called regardless if the event was cancelled.
        /// </summary>
        bool IgnoreCancelled { get; }

        /// <summary>
        ///     The method name.
        /// </summary>
        string Method { get; }

        /// <summary>
        ///     The module name where the event belongs to.
        /// </summary>
        string Module { get; }
    }

}
