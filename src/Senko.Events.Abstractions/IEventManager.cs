using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Senko.Events
{
    public interface IEventManager
    {
        /// <summary>
        ///     Call all the event listeners for the type <see cref="TEvent"/>.
        /// </summary>
        /// <param name="event">The event argument.</param>
        /// <param name="provider">The service provider.</param>
        ValueTask CallAsync<TEvent>(TEvent @event, IServiceProvider provider) where TEvent : IEvent;

        /// <summary>
        ///     Call all the event listeners for the type <see cref="TEvent"/>.
        /// </summary>
        /// <param name="event">The event argument.</param>
        ValueTask CallAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
