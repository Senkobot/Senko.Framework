using System;
using System.Threading.Tasks;

namespace Senko.Events
{
    public interface IEventManager
    {
        /// <summary>
        ///     Returns true if an event with the type <see cref="TEvent"/> is registered.
        /// </summary>
        /// <returns>True if the <see cref="TEvent"/> is registered.</returns>
        bool IsRegistered<TEvent>() where TEvent : IEvent;
        
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
