using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Common;
using Senko.Events.Attributes;
using Senko.Framework;

namespace Senko.Events
{
    internal class RegisteredEventListener : IRegisteredEventListener
    {
        private static readonly ConcurrentDictionary<Type, IRegisteredEventListener[]> Instances = new ConcurrentDictionary<Type, IRegisteredEventListener[]>();
        private readonly Func<object, object, IServiceProvider, Task> _invoker;
        private readonly Type _eventListenerType;

        public RegisteredEventListener(Type eventType, MethodInfo method, EventListenerAttribute attribute, string module, Type eventListenerType)
        {
            EventType = eventType;
            Module = module;
            _eventListenerType = eventListenerType;
            Priority = attribute.Priority;
            PriorityOrder = attribute.PriorityOrder;
            IgnoreCancelled = attribute.IgnoreCancelled;
            Method = method.GetFriendlyName(showParameters: false);
            _invoker = CreateInvoker(method, attribute.IgnoreCancelled);
        }

        public Type EventType { get; }

        public EventPriority Priority { get; }

        public int PriorityOrder { get; set; }

        public bool IgnoreCancelled { get; }

        public string Method { get; }

        public string Module { get; }

        public Task InvokeAsync(object eventHandler, object @event, IServiceProvider provider)
        {
            return _invoker(eventHandler, @event, provider);
        }

        private Func<object, object, IServiceProvider, Task> CreateInvoker(MethodInfo method, bool ignoreCancelled)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var @event = Expression.Parameter(typeof(object), "event");
            var provider = Expression.Parameter(typeof(IServiceProvider), "provider");

            var getRequiredService = typeof(ServiceProviderServiceExtensions)
                .GetMethod("GetRequiredService", new[] {typeof(IServiceProvider)});

            if (getRequiredService == null)
            {
                throw new InvalidOperationException("The method GetRequiredService could not be found.");
            }

            var methodArguments = method.GetParameters();
            var arguments = new Expression[methodArguments.Length];

            for (var i = 0; i < methodArguments.Length; i++)
            {
                var methodArgument = methodArguments[i];

                if (methodArgument.ParameterType == EventType)
                {
                    arguments[i] = Expression.Convert(@event, EventType);
                }
                else
                {
                    arguments[i] = Expression.Call(
                        getRequiredService.MakeGenericMethod(methodArgument.ParameterType), 
                        provider
                    );
                }
            }

            var returnTarget = Expression.Label(typeof(Task));
            Expression invoke = Expression.Call(Expression.Convert(instance, _eventListenerType), method, arguments);

            if (method.ReturnType == typeof(void))
            {
                if (!ignoreCancelled && typeof(IEventCancelable).IsAssignableFrom(EventType))
                {
                    invoke = Expression.Block(
                        Expression.IfThenElse(
                            Expression.Property(@event, nameof(IEventCancelable.IsCancelled)),
                            Expression.Return(returnTarget, Expression.Constant(Task.CompletedTask)),
                            Expression.Block(
                                invoke,
                                Expression.Return(returnTarget, Expression.Constant(Task.CompletedTask))
                            )
                        ),
                        Expression.Label(returnTarget, Expression.Constant(Task.CompletedTask))
                    );
                }
                else
                {
                    invoke = Expression.Block(
                        invoke,
                        Expression.Label(returnTarget, Expression.Constant(Task.CompletedTask))
                    );
                }
            }
            else if (method.ReturnType == typeof(Task))
            {
                if (!ignoreCancelled && typeof(IEventCancelable).IsAssignableFrom(EventType))
                {
                    invoke = Expression.Block(
                        Expression.IfThenElse(
                            Expression.Property(@event, nameof(IEventCancelable.IsCancelled)),
                            Expression.Return(returnTarget, Expression.Constant(Task.CompletedTask)),
                            Expression.Return(returnTarget, invoke)
                        ),
                        Expression.Label(returnTarget, Expression.Constant(Task.CompletedTask))
                    );
                }
            }
            else
            {
                throw new InvalidOperationException($"The method {method.GetFriendlyName()} must return void or Task.");
            }

            return Expression.Lambda<Func<object, object, IServiceProvider, Task>>(invoke,  instance, @event, provider)
                .Compile();
        }

        public static IEnumerable<IRegisteredEventListener> FromType(Type type)
        {
            return Instances.GetOrAdd(type, t =>
            {
                return t.GetMethods()
                    .Where(m => !m.IsStatic && m.GetCustomAttribute(typeof(EventListenerAttribute), false) != null)
                    .SelectMany(m => FromMethod(t, m))
                    .ToArray();
            });
        }

        public static IEnumerable<IRegisteredEventListener> FromMethod(Type listenerType, MethodInfo methodType)
        {
            // Get the return type.
            var returnType = methodType.ReturnType;

            if (returnType != typeof(void) && !returnType.IsTask())
            {
                throw new InvalidOperationException($"The method {methodType.GetFriendlyName()} does not return void or Task.");
            }

            // Register the event.
            var attribute = methodType.GetCustomAttribute<EventListenerAttribute>(false);
            Type[] eventTypes;

            if (attribute.Events.Length == 0)
            {
                if (methodType.GetParameters().Length == 0 || !typeof(IEvent).IsAssignableFrom(methodType.GetParameters()[0].ParameterType))
                {
                    throw new InvalidOperationException($"The first parameter of the method {methodType.GetFriendlyName()} should be the type {nameof(IEvent)}.");
                }

                eventTypes = new[] {methodType.GetParameters()[0].ParameterType};
            }
            else
            {
                eventTypes = attribute.Events;
            }
            
            // Get the module.
            var module = attribute.Module;

            foreach (var eventType in eventTypes)
            {
                var listener = new RegisteredEventListener(eventType, methodType, attribute, module, listenerType);

                yield return listener;
            }
        }
    }
}
