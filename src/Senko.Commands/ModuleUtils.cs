using System;
using System.Collections.Generic;
using System.Reflection;

namespace Senko.Commands
{
    internal static class ModuleUtils
    {
        /// <summary>
        ///     Get the methods that have the attribute <see cref="CommandAttribute"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The methods with their ID.</returns>
        public static IEnumerable<(string id, IReadOnlyList<string> aliases, MethodInfo method)> GetMethods(Type type)
        {
            foreach (var method in type.GetMethods())
            {
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                var commandAliasesAttribute = method.GetCustomAttribute<Alias>();

                if (commandAttribute == null)
                {
                    continue;
                }

                var id = commandAttribute.Id;
                var aliases = commandAliasesAttribute?.Aliases ?? Array.Empty<string>();

                if (string.IsNullOrEmpty(id))
                {
                    throw new InvalidOperationException($"The method {method.Name} has an invalid identifier.");
                }

                yield return (id, aliases, method);
            }
        }

        public static string GetModuleName(MemberInfo moduleType)
        {
            const string moduleSuffix = "Module";

            foreach (var attribute in moduleType.GetCustomAttributes<ModuleAttribute>())
            {
                if (attribute.Name != null)
                {
                    return attribute.Name;
                }
            }

            var moduleName = moduleType.Name;

            if (moduleName.EndsWith(moduleSuffix))
            {
                moduleName = moduleName.Substring(0, moduleName.Length - moduleSuffix.Length);
            }

            return moduleName;
        }

        /// <summary>
        ///     Get the permission name for the given <see cref="moduleType"/> and <see cref="method"/>.
        /// </summary>
        /// <param name="moduleType">The type type.</param>
        /// <param name="method">The method.</param>
        /// <returns>The permission name.</returns>
        public static string GetPermissionName(MemberInfo moduleType, MemberInfo method)
        {
            const string asyncPrefix = "Async";

            var commandName = method.GetCustomAttribute<PermissionAttribute>()?.Name;

            // Try to get the command ID.
            if (string.IsNullOrEmpty(commandName))
            {
                commandName = method.GetCustomAttribute<CommandAttribute>()?.Id;
            }

            // If the attribute contains both the type and command (e.g. type.command) return the attribute value.
            if (commandName != null && commandName.Contains('.'))
            {
                return commandName.ToLower();
            }

            // Otherwise create the permission name.
            if (string.IsNullOrEmpty(commandName))
            {
                commandName = method.Name;
                
                if (commandName.EndsWith(asyncPrefix))
                {
                    commandName = commandName.Substring(0, commandName.Length - asyncPrefix.Length);
                }
            }

            return GetModuleName(moduleType).ToLower() + '.' + commandName.ToLower();
        }
    }
}
