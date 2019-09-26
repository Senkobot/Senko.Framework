using System;
using System.Threading.Tasks;
using Senko.Framework;

namespace Senko.Events
{
    internal interface IRegisteredEventListener<in TEvent> : IRegisteredEventListener
    {
        /// <summary>
        ///     Invoke the method.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="event">The event.</param>
        /// <param name="context">The context.</param>
        /// <param name="provider">The service provider.</param>
        Task InvokeAsync(object eventHandler, TEvent @event, MessageContext context, IServiceProvider provider);
    }

}