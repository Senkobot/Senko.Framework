using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Senko.Commands.Reflection
{
    public class ReflectionModuleCompiler : IModuleCompiler
    {
        private readonly ILogger<ReflectionModuleCompiler> _logger;
        private readonly IReadOnlyList<ICommandValueProvider> _valueProviders;

        public ReflectionModuleCompiler(ILogger<ReflectionModuleCompiler> logger, IEnumerable<ICommandValueProvider> valueProviders)
        {
            _logger = logger;
            _valueProviders = valueProviders.ToList();
        }

        public IEnumerable<ICommand> Compile(IEnumerable<Type> typesEnumerable)
        {
            var modules = typesEnumerable as Type[] ?? typesEnumerable.ToArray();
            var stopwatch = Stopwatch.StartNew();
            var commands = modules
                .SelectMany(t => ModuleUtils.GetMethods(t).Select(m => new ReflectionCommand(m.id, m.aliases, t, m.method, _valueProviders)))
                .ToArray();
            
            _logger.LogTrace("Compiled {CommandCount} commands from {ModuleCount} modules in {Duration:0.00} ms.", commands.Length, modules.Length, stopwatch.Elapsed.TotalMilliseconds);
            
            return commands;
        }
    }
}
