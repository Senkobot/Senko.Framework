using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Events;
using Senko.Localization;

namespace Senko.Commands.Managers
{
    public class CommandManager : ICommandManager
    {
        private readonly IModuleManager _moduleManager;
        private readonly IServiceProvider _provider;
        private readonly IModuleCompiler _moduleCompiler;
        private readonly IStringLocalizer _localizer;
        private IDictionary<CultureInfo, IReadOnlyDictionary<string, ICommand[]>> _commandsByCultureId;
        private IDictionary<CultureInfo, IReadOnlyDictionary<string, string>> _idToNames;
        private IDictionary<string, ICommand[]> _commandsById;
        private IReadOnlyList<ICommand> _commands;
        private readonly ILogger<CommandManager> _logger;
        private bool _initialized;

        public CommandManager(
            IModuleCompiler moduleCompiler,
            IStringLocalizer localizer,
            ILogger<CommandManager> logger,
            IServiceProvider provider,
            IModuleManager moduleManager)
        {
            _moduleCompiler = moduleCompiler;
            _localizer = localizer;
            _logger = logger;
            _provider = provider;
            _moduleManager = moduleManager;
        }

        public virtual IReadOnlyList<ICommand> Commands
        {
            get
            {
                Initialize();
                return _commands;
            }
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            var commands = _provider.GetServices<ICommand>();
            var types = _moduleManager.ModuleTypes;

            _commandsByCultureId = new Dictionary<CultureInfo, IReadOnlyDictionary<string, ICommand[]>>();
            _idToNames = new Dictionary<CultureInfo, IReadOnlyDictionary<string, string>>();

            var allCommands = commands.Union(_moduleCompiler.Compile(types)).ToArray();

            _commands = allCommands;
            _commandsById = allCommands
                .SelectMany(c => new [] { c.Id }.Union(c.Aliases).Select(id => new KeyValuePair<string,ICommand>(id, c)))
                .GroupBy(c => c.Key)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToArray());

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
        }

        public virtual IReadOnlyCollection<ICommand> FindAll(string name, CultureInfo culture = null)
        {
            Initialize();

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