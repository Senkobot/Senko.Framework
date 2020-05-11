using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Framework;

namespace Senko.Events
{
    public class EventManager : IEventManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageContextAccessor _contextAccessor;

        public EventManager(IServiceProvider serviceProvider, IMessageContextAccessor contextAccessor)
        {
            _serviceProvider = serviceProvider;
            _contextAccessor = contextAccessor;
        }

        protected virtual Task<IReadOnlyCollection<string>> GetEnabledModulesAsync(IEvent @event)
        {
            return Task.FromResult<IReadOnlyCollection<string>>(null);
        }

        /// <summary>
        ///     Get all the event listeners for the given event type.
        /// </summary>
        /// <param name="services">Current service provider.</param>
        /// <param name="modules">Active modules for the current guild..</param>
        /// <returns>The event listeners.</returns>
        private static IEnumerable<EventHandler> GetHandlers<TEvent>(
            IServiceProvider services,
            IReadOnlyCollection<string> modules = null)
            where TEvent : IEvent
        {
            var checkEnabledModule = modules != null;

            foreach (var handler in services.GetServices<IEventListener>())
            {
                var events = RegisteredEventListener.FromType(handler.GetType());

                foreach (var eventHandler in events)
                {
                    if (eventHandler.EventType != typeof(TEvent)
                        || checkEnabledModule
                        && eventHandler.Module != null
                        && !modules.Contains(eventHandler.Module))
                    {
                        continue;
                    }

                    yield return new EventHandler(handler, eventHandler);
                }
            }
        }

        /// <inheritdoc />
        public bool IsRegistered<TEvent>() where TEvent : IEvent
        {
            using var scope = _serviceProvider.CreateScope();

            return GetHandlers<TEvent>(_serviceProvider).Any();
        }

        /// <inheritdoc />
        public async ValueTask CallAsync<TEvent>(TEvent @event, IServiceProvider services) where TEvent : IEvent
        {
            var enabledModules = await GetEnabledModulesAsync(@event);

            foreach (var (handler, eventListener) in GetHandlers<TEvent>(services, enabledModules))
            {
                await eventListener.InvokeAsync(handler, @event, services);
            }
        }

        /// <inheritdoc />
        public async ValueTask CallAsync<T>(T @event) where T : IEvent
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
        }
    }
}
