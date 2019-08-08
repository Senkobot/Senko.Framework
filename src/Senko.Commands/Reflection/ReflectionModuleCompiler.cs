using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Senko.Commands.Reflection
{
    public class ReflectionModuleCompiler : IModuleCompiler
    {
        private readonly ILogger<ReflectionModuleCompiler> _logger;

        public ReflectionModuleCompiler(ILogger<ReflectionModuleCompiler> logger)
        {
            _logger = logger;
        }

        public IEnumerable<ICommand> Compile(IEnumerable<IModule> modulesEnumerable)
        {
            var modules = modulesEnumerable as IModule[] ?? modulesEnumerable.ToArray();
            var stopwatch = Stopwatch.StartNew();
            var commands = modules.SelectMany(m => ModuleUtils.GetMethods(m).Select(t => new ReflectionCommand(t.id, m, t.method))).ToArray();
            
            _logger.LogTrace("Compiled {CommandCount} commands from {ModuleCount} modules in {Duration:0.00} ms.", commands.Length, modules.Length, stopwatch.Elapsed.TotalMilliseconds);
            
            return commands;
        }
    }
}
