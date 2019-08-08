using System;
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
    internal class RegisteredEventListener<TListener, TEvent> : IRegisteredEventListener<TEvent>
    {
        private readonly Func<TEvent, MessageContext, IServiceProvider, Task> _invoker;

        public RegisteredEventListener(Type type, TListener listener, MethodInfo method, EventListenerAttribute attribute, string module)
        {
            Type = type;
            Listener = listener;
            Module = module;
            Priority = attribute.Priority;
            PriorityOrder = attribute.PriorityOrder;
            IgnoreCancelled = attribute.IgnoreCancelled;
            Method = method.GetFriendlyName(showParameters: false);
            _invoker = CreateInvoker(listener, method, attribute.IgnoreCancelled);
        }

        public Type Type { get; }

        public TListener Listener { get; }

        public EventPriority Priority { get; }

        public int PriorityOrder { get; set; }

        public bool IgnoreCancelled { get; }

        public string Method { get; }

        public string Module { get; }

        public Task InvokeAsync(TEvent @event, MessageContext context, IServiceProvider provider)
        {
            return _invoker(@event, context, provider);
        }

        private static Func<TEvent, MessageContext, IServiceProvider, Task> CreateInvoker(TListener listener,
            MethodInfo method, bool ignoreCancelled)
        {
            var @event = Expression.Parameter(typeof(TEvent), "event");
            var context = Expression.Parameter(typeof(MessageContext), "context");
            var provider = Expression.Parameter(typeof(IServiceProvider), "provider");

            var getRequiredService = typeof(ServiceProviderServiceExtensions)
                .GetMethod("GetRequiredService", new[] {typeof(IServiceProvider)});

            if (getRequiredService == null)
            {
                throw new InvalidOperationException("The method GetRequiredService could not be found.");
            }

            var instance = Expression.Constant(listener);
            var methodArguments = method.GetParameters();
            var arguments = new Expression[methodArguments.Length];

            for (var i = 0; i < methodArguments.Length; i++)
            {
                var methodArgument = methodArguments[i];

                if (methodArgument.ParameterType == typeof(TEvent))
                {
                    arguments[i] = @event;
                }
                else if (methodArgument.ParameterType == typeof(MessageContext))
                {
                    arguments[i] = context;
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
            Expression invoke = Expression.Call(instance, method, arguments);

            if (method.ReturnType == typeof(void))
            {
                if (!ignoreCancelled && typeof(IEventCancelable).IsAssignableFrom(typeof(TEvent)))
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
                if (!ignoreCancelled && typeof(IEventCancelable).IsAssignableFrom(typeof(TEvent)))
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

            return Expression.Lambda<Func<TEvent, MessageContext, IServiceProvider, Task>>(invoke, @event, context, provider)
                .Compile();
        }
    }

    internal static class RegisteredEventListener
    {
        public static IEnumerable<IRegisteredEventListener> FromInstance(IEventListener instance)
        {
            return instance
                .GetType()
                .GetMethods()
                .Where(m => !m.IsStatic && m.GetCustomAttribute(typeof(EventListenerAttribute), false) != null)
                .SelectMany(m => FromMethod(instance, m));
        }

        public static IEnumerable<IRegisteredEventListener> FromMethod(IEventListener instance, MethodInfo methodType)
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

            if (module == null && instance.GetType().GetInterfaces().Any(t => t.Name == "IModule"))
            {
                module = GetModuleName(instance.GetType());
            }

            foreach (var eventType in eventTypes)
            {
                var constructor = typeof(RegisteredEventListener<,>).MakeGenericType(methodType.DeclaringType, eventType);
                var listener = (IRegisteredEventListener)Activator.CreateInstance(constructor, eventType, instance, methodType, attribute, module);

                yield return listener;
            }
        }

        private static string GetModuleName(MemberInfo moduleType)
        {
            const string moduleSuffix = "Module";
            var moduleName = moduleType.Name;

            if (moduleName.EndsWith(moduleSuffix))
            {
                moduleName = moduleName.Substring(0, moduleName.Length - moduleSuffix.Length);
            }

            return moduleName;
        }
    }
}
