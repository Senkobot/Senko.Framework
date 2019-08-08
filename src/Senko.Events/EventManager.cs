using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Framework;

namespace Senko.Events
{
    public class EventManager : IEventManager
    {
        private readonly ConcurrentDictionary<Type, object> _cachedArrays;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageContextAccessor _contextAccessor;

        public EventManager(IServiceProvider serviceProvider, IMessageContextAccessor contextAccessor)
        {
            _serviceProvider = serviceProvider;
            _cachedArrays = new ConcurrentDictionary<Type, object>();
            _contextAccessor = contextAccessor;
        }

        /// <inheritdoc />
        public IEnumerable<IRegisteredEventListener> GetEventListeners()
        {
            var collection = _serviceProvider.GetRequiredService<EventListenerCollection>();

            return collection.GetAll();
        }

        /// <inheritdoc />
        public async Task<TEvent> CallAsync<TEvent>(TEvent @event, IServiceProvider services) where TEvent : IEvent
        {
            // Get the events.
            var listeners = (IRegisteredEventListener<TEvent>[]) _cachedArrays.GetOrAdd(@event.GetType(), type =>
            {
                var collection = _serviceProvider.GetRequiredService<EventListenerCollection>();

                return collection.GetAll<TEvent>()
                    .OrderByDescending(e => e.Priority)
                    .ThenByDescending(e => e.PriorityOrder)
                    .ToArray();
            });

            if (listeners.Length == 0)
            {
                return @event;
            }

            var context = _contextAccessor?.Context;
            var enabledModules = await GetEnabledModulesAsync(@event);
            var checkEnabledModule = enabledModules != null;

            foreach (var eventData in listeners)
            {
                if (checkEnabledModule && eventData.Module != null && !enabledModules.Contains(eventData.Module))
                {
                    continue;
                }

                await eventData.InvokeAsync(@event, context, services);
            }

            return @event;
        }

        protected virtual Task<IReadOnlyCollection<string>> GetEnabledModulesAsync(IEvent @event)
        {
            return Task.FromResult<IReadOnlyCollection<string>>(null);
        }

        /// <inheritdoc />
        public async Task<T> CallAsync<T>(T @event) where T : IEvent
        {
            var services = _contextAccessor?.Context?.RequestServices;

            if (services != null)
            {
                await CallAsync(@event, services);
            }
            else
            {
                var scope = _serviceProvider.CreateScope();

                try
                {
                    await CallAsync(@event, scope.ServiceProvider);
                }
                finally 
                {
                    scope?.Dispose();
                }
            }
            
            return @event;
        }
    }
}
