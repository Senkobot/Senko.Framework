using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Events;
using Senko.Localizations;

namespace Senko.Commands.Managers
{
    public class CommandManager : ICommandManager, IEventListener
    {
        private readonly IServiceProvider _provider;
        private readonly IModuleCompiler _moduleCompiler;
        private readonly IStringLocalizer _localizer;
        private IDictionary<CultureInfo, IReadOnlyDictionary<string, ICommand[]>> _commandsByCultureId;
        private IDictionary<CultureInfo, IReadOnlyDictionary<string, string>> _idToNames;
        private IDictionary<string, ICommand[]> _commandsById;
        private readonly ILogger<CommandManager> _logger;

        public CommandManager(
            IModuleCompiler moduleCompiler,
            IStringLocalizer localizer,
            ILogger<CommandManager> logger,
            IServiceProvider provider
        )
        {
            _moduleCompiler = moduleCompiler;
            _localizer = localizer;
            _logger = logger;
            _provider = provider;
        }

        public virtual IReadOnlyList<ICommand> Commands { get; private set; } = Array.Empty<ICommand>();

        [EventListener(typeof(InitializeEvent), EventPriority.High, PriorityOrder = 300)]
        public virtual Task InitializeAsync()
        {
            var commands = _provider.GetServices<ICommand>();
            var modules = _provider.GetServices<IModule>();

            _commandsByCultureId = new Dictionary<CultureInfo, IReadOnlyDictionary<string, ICommand[]>>();
            _idToNames = new Dictionary<CultureInfo, IReadOnlyDictionary<string, string>>();

            var allCommands = commands.Union(_moduleCompiler.Compile(modules)).ToArray();

            Commands = allCommands;
            _commandsById = allCommands
                .GroupBy(c => c.Id)
                .ToDictionary(g => g.Key, g => g.ToArray());

            foreach (var culture in _localizer.Cultures)
            {
                var missingIds = new List<string>();
                var commandsById = _commandsById.Keys.ToDictionary(
                    id => id,
                    id =>
                    {
                        if (_localizer.TryGetString("Command." + id + ".Name", culture, out var value))
                        {
                            return value.ToString().ToLower();
                        }

                        missingIds.Add(id);
                        return id;
                    }
                );

                _idToNames.Add(culture, commandsById);
                _commandsByCultureId.Add(culture, commandsById.ToDictionary(kv => kv.Value, kv => _commandsById[kv.Key]));

                _logger.LogDebug($"The culture {culture} is missing the following command names: {string.Join(", ", missingIds)}");
            }

            return Task.CompletedTask;
        }

        public virtual IReadOnlyCollection<ICommand> FindAll(string name, CultureInfo culture = null)
        {
            IReadOnlyCollection<ICommand> commands;

            if (_commandsByCultureId.TryGetValue(culture ?? CultureInfo.CurrentCulture, out var commandByIds)
                && commandByIds.TryGetValue(name, out var commandsByCulture))
            {
                commands = commandsByCulture;
            }
            else if (_commandsById.TryGetValue(name, out var commandsById))
            {
                commands = commandsById;
            }
            else
            {
                commands = new ICommand[0];
            }

            return commands;
        }

        public string GetName(string id, CultureInfo culture = null)
        {
            return _idToNames.TryGetValue(culture ?? CultureInfo.CurrentCulture, out var idToNames)
                   && idToNames.TryGetValue(id, out var name)
                ? name
                : id;
        }
    }
}