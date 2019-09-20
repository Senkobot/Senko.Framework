﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Senko.Commands.Roslyn
{
    public class RoslynModuleCompiler : IModuleCompiler
    {
        private readonly ILogger<RoslynModuleCompiler> _logger;
        private readonly IReadOnlyList<ICommandValueProvider> _valueProviders;

        public RoslynModuleCompiler(ILogger<RoslynModuleCompiler> logger, IEnumerable<ICommandValueProvider> valueProviders)
        {
            _logger = logger;
            _valueProviders = valueProviders.ToList();
        }

        public IEnumerable<ICommand> Compile(IEnumerable<IModule> modulesEnumerable)
        {
            var modules = modulesEnumerable as IModule[] ?? modulesEnumerable.ToArray();
            var compiler = new RoslynCommandBuilder(_valueProviders);
            var stopwatch = Stopwatch.StartNew();

            compiler.AddModules(modules);

            var commands = compiler.Compile().ToArray();

            _logger.LogTrace("Compiled {CommandCount} commands from {ModuleCount} modules in {Duration:0.00} ms.", commands.Length, modules.Length, stopwatch.Elapsed.TotalMilliseconds);
            
            return commands;
        }
    }
}