using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Senko.Events
{
    public class EventListenerSource<T> : IEventListenerSource
    {
        private readonly IServiceProvider _provider;

        public EventListenerSource(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<IEventListener> GetEventListeners()
        {
            return _provider.GetServices<T>().OfType<IEventListener>();
        }
    }
}
