using System;

namespace Senko.Events
{
    public static class EventExtensions
    {
        /// <summary>
        ///     Cancel the <see cref="IEvent"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the event is not cancelable.</exception>
        /// <param name="args"></param>
        public static void Cancel(this IEvent args)
        {
            if (!(args is IEventCancelable cancelable))
            {
                throw new InvalidOperationException("The event is not cancelable.");
            }

            cancelable.IsCancelled = true;
        }
    }
}
