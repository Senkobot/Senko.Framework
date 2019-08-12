using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Senko.Events
{
    public class EventListenerCollection
    {
        private readonly IServiceProvider _provider;
        private IRegisteredEventListener[] _eventListeners;

        public EventListenerCollection(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IRegisteredEventListener[] EventListeners
            => _eventListeners ??= GetListeners();

        private IRegisteredEventListener[] GetListeners()
        {
            return _provider
                .GetServices<IEventListener>()
                .Concat(_provider.GetServices<IEventListenerSource>().SelectMany(s => s.GetEventListeners()))
                .Distinct()
                .SelectMany(RegisteredEventListener.FromInstance)
                .ToArray();
        }

        public IEnumerable<IRegisteredEventListener> GetAll()
        {
            return EventListeners;
        }

        internal IEnumerable<IRegisteredEventListener<T>> GetAll<T>()
        {
            return EventListeners
                .OfType<IRegisteredEventListener<T>>()
                .ToArray();
        }
    }
}
