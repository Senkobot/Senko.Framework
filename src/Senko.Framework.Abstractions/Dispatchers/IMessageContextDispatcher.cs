using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace Senko.Framework
{
    public interface IMessageContextDispatcher
    {
        /// <summary>
        /// Sends the response of the <see cref="MessageContext"/> to Discord.
        /// </summary>
        /// <param name="context">The context to be sent.</param>
        ValueTask DispatchAsync(MessageContext context);

        /// <summary>
        /// Create a new message context, execute the <see cref="func"/> and send the response of the <see cref="MessageContext"/> to Discord. 
        /// </summary>
        /// <param name="func">The action.</param>
        /// <param name="features">The features of the context.</param>
        Task DispatchAsync(Func<MessageContext, Task> func, FeatureCollection features = null);
    }
}
