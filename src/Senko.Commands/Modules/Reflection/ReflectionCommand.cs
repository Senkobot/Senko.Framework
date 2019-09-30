using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Arguments;
using Senko.Framework;

namespace Senko.Commands.Reflection
{
    public class ReflectionCommand : ICommand
    {
        private readonly MethodInfo _method;
        private readonly IReadOnlyList<ICommandValueProvider> _valueProviders;
        private readonly Type _moduleType;

        public ReflectionCommand(
            string id,
            IReadOnlyList<string> aliases,
            Type moduleType,
            MethodInfo method,
            IReadOnlyList<ICommandValueProvider> valueProviders)
        {
            var attribute = method.GetCustomAttribute<CommandAttribute>();

            Id = id;
            _moduleType = moduleType;
            _method = method;
            _valueProviders = valueProviders;
            Aliases = aliases;
            Module = ModuleUtils.GetModuleName(moduleType);
            Permission = ModuleUtils.GetPermissionName(moduleType, method);
            GuildOnly = attribute.GuildOnly;
            PermissionGroup = attribute.PermissionGroup;
        }

        public string Id { get; }

        public IReadOnlyList<string> Aliases { get; }

        public string Module { get; }

        public string Permission { get; }

        public PermissionGroup PermissionGroup { get; }

        public bool GuildOnly { get; }

        public async Task ExecuteAsync(MessageContext context)
        {
            object module;
            var constructor = _moduleType.GetConstructors().FirstOrDefault();

            if (constructor == null)
            {
                module = Activator.CreateInstance(_moduleType);
            }
            else
            {
                var constructorParameters = constructor.GetParameters();
                var constructorArgs = new object[constructorParameters.Length];

                for (var i = 0; i < constructorParameters.Length; i++)
                {
                    constructorArgs[i] = await GetValue(context, constructorParameters[i]);
                }

                module = constructor.Invoke(constructorArgs);
            }

            var contextProperty = _moduleType
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes<ModuleContextAttribute>().Any());

            if (contextProperty != null)
            {
                contextProperty.SetValue(module, new ModuleContext
                {
                    Context = context
                });
            }

            var methodParameters = _method.GetParameters();
            var methodArgs = new object[methodParameters.Length];

            for (var i = 0; i < methodParameters.Length; i++)
            {
                methodArgs[i] = await GetValue(context, methodParameters[i]);
            }

            var result = _method.Invoke(module, methodArgs.ToArray());

            if (result is Task resultTask)
            {
                await resultTask;
            }
            else if (result is ValueTask resultValueTask)
            {
                await resultValueTask;
            }
        }

        private async Task<object> GetValue(MessageContext context, ParameterInfo parameter)
        {
            var argType = parameter.ParameterType;
            var isUnsafe = parameter.GetCustomAttributes<UnsafeAttribute>().Any();
            var name = parameter.Name;
            var required = !parameter.GetCustomAttributes<OptionalAttribute>().Any();

            if (argType == typeof(MessageContext))
            {
                return context;
            }
            if (argType == typeof(MessageResponse))
            {
                return context.Response;
            }
            if (argType == typeof(MessageRequest))
            {
                return context.RequestServices;
            }
            if (argType == typeof(IDiscordGuild))
            {
                return await context.Request.GetGuildAsync();
            }
            if (argType == typeof(IDiscordUser))
            {
                return await context.Request.ReadUserMentionAsync(name, required);
            }
            if (argType == typeof(IDiscordGuildUser))
            {
                return await context.Request.ReadGuildUserMentionAsync(name, required);
            }
            if (argType == typeof(IDiscordRole))
            {
                return await context.Request.ReadRoleMentionAsync(name, required);
            }
            if (argType == typeof(IDiscordChannel))
            {
                return await context.Request.ReadGuildChannelAsync(name, required);
            }
            if (argType == typeof(int))
            {
                return context.Request.ReadInt32(name, required);
            }
            if (argType == typeof(uint))
            {
                return context.Request.ReadUInt32(name, required);
            }
            if (argType == typeof(long))
            {
                return context.Request.ReadInt64(name, required);
            }
            if (argType == typeof(ulong))
            {
                return context.Request.ReadUInt64(name, required);
            }
            if (argType == typeof(string))
            {
                var escapeTypes = parameter.GetCustomAttribute<EscapeAttribute>()?.Type ?? EscapeType.Default;

                if (parameter.GetCustomAttributes<RemainingAttribute>().Any())
                {
                    if (isUnsafe)
                    {
                        return context.Request.ReadUnsafeRemaining(name, required);
                    }

                    return await context.Request.ReadRemainingAsync(name, required, escapeTypes);
                }

                if (isUnsafe)
                {
                    return context.Request.ReadUnsafeString(name, required);
                }

                return await context.Request.ReadStringAsync(name, required, escapeTypes);
            }

            foreach (var provider in _valueProviders)
            {
                if (provider.CanProvide(argType))
                {
                    return await provider.GetValueAsync(parameter, context);
                }
            }

            return context.RequestServices.GetService(argType);
        }
    }
}
