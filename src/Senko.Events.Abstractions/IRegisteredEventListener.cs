using System;
using System.Threading.Tasks;

namespace Senko.Events
{
    public interface IRegisteredEventListener
    {
        /// <summary>
        ///     The type that the event is listening to.
        /// </summary>
        Type EventType { get; }

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

        /// <summary>
        ///     Invoke the method.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="event">The event.</param>
        /// <param name="provider">The service provider.</param>
        Task InvokeAsync(object eventHandler, object @event, IServiceProvider provider);
    }

}
