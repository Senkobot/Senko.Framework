﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Senko.Commands
{
    internal static class ModuleUtils
    {
        /// <summary>
        ///     Get the methods that have the attribute <see cref="CommandAttribute"/>.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>The methods with their ID.</returns>
        public static IEnumerable<(string id, MethodInfo method)> GetMethods(IModule module)
        {
            foreach (var method in module.GetType().GetMethods())
            {
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();

                if (commandAttribute == null)
                {
                    continue;
                }

                var id = commandAttribute.Id;

                if (string.IsNullOrEmpty(id))
                {
                    throw new InvalidOperationException($"The method {method.Name} has an invalid identifier.");
                }

                yield return (id, method);
            }
        }

        public static string GetModuleName(MemberInfo moduleType)
        {
            const string moduleSuffix = "Module";
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
        /// <param name="moduleType">The module type.</param>
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

            // If the attribute contains both the module and command (e.g. module.command) return the attribute value.
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
