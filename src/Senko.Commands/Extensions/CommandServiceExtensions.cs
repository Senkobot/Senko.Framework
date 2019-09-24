﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Senko.Commands.Events;
using Senko.Commands.Managers;
using Senko.Commands.Reflection;
using Senko.Commands.Roslyn;
using Senko.Events;
using Senko.Localization;
using Senko.Localization.Resources;

namespace Senko.Commands
{
    public class CommandBuilder : ICommandBuilder
    {
        public List<Type> Modules { get; set; } = new List<Type>();

        public ICommandBuilder AddModule(Type type)
        {
            Modules.Add(type);
            return this;
        }

        public ICommandBuilder AddModules(Assembly assembly)
        {
            var newModules = assembly
                .GetTypes()
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && t.GetMethods().Any(m => m.GetCustomAttributes<CommandAttribute>().Any()));

            foreach (var type in newModules)
            {
                Modules.Add(type);
            }

            return this;
        }
    }

    public static class CommandServiceExtensions
    {
        public static ICommandBuilder AddCommand(this IServiceCollection services)
        {
            var commandBuilder = new CommandBuilder();
            
            // Event
            services.AddSingleton<IEventManager, ModuleEventManager>();
            services.AddSingleton<EventListenerCollection>();

            // Module
            services.AddSingleton<IModuleManager>(provider =>
            {
                var instance = ActivatorUtilities.CreateInstance<ModuleManager>(provider);
                instance.Initialize(commandBuilder.Modules);
                return instance;
            });

            services.TryAddSingleton<IModuleCompiler>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ModuleOptions>>();

                switch (options.Value.Compiler.ToLowerInvariant())
                {
                    case "reflection":
                        return ActivatorUtilities.CreateInstance<ReflectionModuleCompiler>(provider);
                    case "roslyn":
                        return ActivatorUtilities.CreateInstance<RoslynModuleCompiler>(provider);
                    default:
                        throw new NotSupportedException($"The compiler {options.Value.Compiler} is unknown.");
                }
            });

            // Permission
            services.AddSingleton<IPermissionManager, PermissionManager>();

            // Command
            services.AddSingleton<ICommandManager, CommandManager>();
            services.AddSingleton<IStringRepository>(new ResourceStringRepository(typeof(CommandManager).Assembly));

            return commandBuilder;
        }
    }
}
