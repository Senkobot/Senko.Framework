using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Arguments;
using Senko.Common;
using Senko.Framework;

namespace Senko.Commands.Reflection
{
    public class ReflectionCommand : ICommand
    {
        private readonly MethodInfo _method;
        private readonly object _module;

        public ReflectionCommand(string id, object module, MethodInfo method)
        {
            var attribute = method.GetCustomAttribute<CommandAttribute>();

            Id = id;
            _module = module;
            _method = method;
            Module = ModuleUtils.GetModuleName(module.GetType());
            Permission = ModuleUtils.GetPermissionName(module.GetType(), method);
            GuildOnly = attribute.GuildOnly;
            PermissionGroup = attribute.PermissionGroup;
        }

        public string Id { get; }

        public string Module { get; }

        public string Permission { get; }

        public PermissionGroup PermissionGroup { get; }

        public bool GuildOnly { get; }

        public async Task ExecuteAsync(MessageContext context)
        {
            var args = new List<object>();

            foreach (var parameter in _method.GetParameters())
            {
                var argType = parameter.ParameterType;
                var isUnsafe = parameter.GetCustomAttributes<UnsafeAttribute>().Any();
                var name = parameter.Name;
                var required = !parameter.GetCustomAttributes<OptionalAttribute>().Any();

                object arg;

                if (argType == typeof(MessageContext))
                {
                    arg = context;
                }
                else if (argType == typeof(MessageResponse))
                {
                    arg = context.Response;
                }
                else if (argType == typeof(MessageRequest))
                {
                    arg = context.RequestServices;
                }
                else if (argType == typeof(IDiscordGuild))
                {
                    arg = await context.Request.GetGuildAsync();
                }
                else if (argType == typeof(IDiscordUser))
                {
                    arg = await context.Request.ReadUserMentionAsync(name, required);
                }
                else if (argType == typeof(IDiscordGuildUser))
                {
                    arg = await context.Request.ReadGuildUserMentionAsync(name, required);
                }
                else if (argType == typeof(IDiscordRole))
                {
                    arg = await context.Request.ReadRoleMentionAsync(name, required);
                }
                else if (argType == typeof(IDiscordChannel))
                {
                    arg = await context.Request.ReadGuildChannelAsync(name, required);
                }
                else if (argType == typeof(int))
                {
                    arg = context.Request.ReadInt32(name, required);
                }
                else if (argType == typeof(uint))
                {
                    arg = context.Request.ReadUInt32(name, required);
                }
                else if (argType == typeof(long))
                {
                    arg = context.Request.ReadInt64(name, required);
                }
                else if (argType == typeof(ulong))
                {
                    arg = context.Request.ReadUInt64(name, required);
                }
                else if (argType == typeof(string))
                {
                    var escapeTypes = parameter.GetCustomAttribute<EscapeAttribute>()?.Type ?? EscapeType.Default;

                    if (parameter.GetCustomAttributes<RemainingAttribute>().Any())
                    {
                        if (isUnsafe)
                        {
                            arg = context.Request.ReadUnsafeRemaining(name, required);
                        }
                        else
                        {
                            arg = await context.Request.ReadRemainingAsync(name, required, escapeTypes);
                        }
                    }
                    else
                    {
                        if (isUnsafe)
                        {
                            arg = context.Request.ReadUnsafeString(name, required);
                        }
                        else
                        {
                            arg = await context.Request.ReadStringAsync(name, required, escapeTypes);
                        }
                    }
                }
                else
                {
                    arg = context.RequestServices.GetService(argType);
                }

                args.Add(arg);
            }

            var result = _method.Invoke(_module, args.ToArray());

            if (result is Task resultTask)
            {
                await resultTask;
            }
        }
    }
}
