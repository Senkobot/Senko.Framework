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

        /// <inheritdoc />
        public async ValueTask CallAsync<TEvent>(TEvent @event, IServiceProvider services) where TEvent : IEvent
        {
            var context = _contextAccessor?.Context;
            var enabledModules = await GetEnabledModulesAsync(@event);
            var checkEnabledModule = enabledModules != null;

            foreach (var handler in services.GetServices<IEventListener>())
            {
                var events = RegisteredEventListener.FromType(handler.GetType());

                foreach (var eventHandler in events)
                {
                    if (eventHandler.EventType != typeof(TEvent)
                        || checkEnabledModule
                        && eventHandler.Module != null
                        && !enabledModules.Contains(eventHandler.Module))
                    {
                        continue;
                    }

                    await eventHandler.InvokeAsync(handler, @event, services);
                }
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
