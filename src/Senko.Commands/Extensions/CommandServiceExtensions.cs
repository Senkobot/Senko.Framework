using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Senko.Arguments;
using Senko.Commands.Events;
using Senko.Commands.Managers;
using Senko.Commands.Reflection;
using Senko.Commands.Roslyn;
using Senko.Events;
using Senko.Framework;
using Senko.Localization;
using Senko.Localization.Resources;

namespace Senko.Commands
{
    public static class CommandServiceExtensions
    {
        public static IServiceCollection AddCommand(this IServiceCollection services)
        {
            // Event
            services.AddSingleton<IEventManager, ModuleEventManager>();
            services.AddSingleton<EventListenerCollection>();

            // Module
            services.AddSingleton<IModuleManager, ModuleManager>();
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

            return services;
        }

        public static IServiceCollection AddModule<TModule>(this IServiceCollection services) where TModule : class, IModule
        {
            return services.AddSingleton<IModule, TModule>();
        }

        public static IServiceCollection AddModules(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract))
            {
                services.AddSingleton(typeof(IModule), type);
            }

            return services;
        }
    }
}
