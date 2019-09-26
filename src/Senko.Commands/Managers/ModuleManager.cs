using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.DependencyInjection;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;
using Senko.Common;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Events;

namespace Senko.Commands.Managers
{
    public class ModuleManager : IModuleManager
    {
        private readonly IServiceProvider _provider;
        private readonly ICacheClient _cache;

        public ModuleManager(ICacheClient cache, IServiceProvider provider)
        {
            _cache = cache;
            _provider = provider;
        }

        public static TimeSpan CacheTime { get; set; } = TimeSpan.FromMinutes(5);

        public IReadOnlyList<Type> ModuleTypes { get; private set; } = Array.Empty<Type>();

        public virtual IReadOnlyList<string> ModuleNames { get; private set; } = Array.Empty<string>();

        public virtual IReadOnlyList<string> CoreModuleNames { get; private set; } = Array.Empty<string>();

        public virtual IReadOnlyList<string> DefaultModuleNames { get; private set; } = Array.Empty<string>();

        internal void Initialize(IReadOnlyList<Type> moduleTypes)
        {
            ModuleTypes = moduleTypes;

            ModuleNames = moduleTypes
                .Select(ModuleUtils.GetModuleName)
                .ToArray();

            CoreModuleNames = moduleTypes
                .Where(t => t.GetCustomAttributes<CoreModuleAttribute>().Any())
                .Select(ModuleUtils.GetModuleName)
                .ToArray();

            DefaultModuleNames = moduleTypes
                .Where(t => t.GetCustomAttributes<DefaultModuleAttribute>().Any())
                .Select(ModuleUtils.GetModuleName)
                .ToArray();
        }

        public virtual async Task<IReadOnlyCollection<string>> GetEnabledModulesAsync(ulong guildId)
        {
            var cacheKey = CacheKey.GetEnabledModulesCacheKey(guildId);
            var cache = await _cache.GetAsync<string[]>(cacheKey);

            if (cache.HasValue)
            {
                return cache.Value;
            }

            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetService<IGuildModuleRepository>();
            string[] enabledModules;

            if (repo != null)
            {
                var entities = (await repo.GetAllAsync(guildId))
                    .Where(gm => gm.Enabled)
                    .ToDictionary(gm => gm.Name, gm => gm.Enabled, StringComparer.OrdinalIgnoreCase);

                var result = entities
                    .Where(kv => kv.Value)
                    .Select(kv => kv.Key)
                    .ToList();

                // Add the default-enabled modules.
                result.AddRange(DefaultModuleNames.Where(name => !entities.ContainsKey(name)));

                // Add the modules that are always enabled.
                foreach (var name in CoreModuleNames)
                {
                    if (!result.Contains(name, StringComparer.OrdinalIgnoreCase))
                    {
                        result.Add(name);
                    }
                }

                enabledModules = result.ToArray();
            }
            else
            {
                enabledModules = ModuleNames.ToArray();
            }

            await _cache.SetAsync(cacheKey, enabledModules, CacheTime);

            return enabledModules;
        }

        public virtual async Task SetModuleEnabledAsync(ulong guildId, string casedModuleName, bool enabled)
        {
            var moduleName = ModuleNames.FirstOrDefault(name => name.Equals(casedModuleName, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(moduleName))
            {
                throw new KeyNotFoundException("The module could not be found.");
            }

            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetService<IGuildModuleRepository>();

            if (repo == null)
            {
                throw new NotSupportedException($"There was no {nameof(IGuildModuleRepository)} registered. {nameof(SetModuleEnabledAsync)} is not available.");
            }

            var enabledGuildModule = await repo.GetAsync(guildId, moduleName);

            if (CoreModuleNames.Contains(moduleName))
            {
                throw new InvalidOperationException($"The module {moduleName} is a core module and cannot be disabled");
            }

            if (enabledGuildModule == null)
            {
                enabledGuildModule = new GuildModule
                {
                    GuildId = guildId,
                    Name = moduleName,
                    Enabled = enabled
                };
                repo.Add(enabledGuildModule);
            } 
            else
            {
                enabledGuildModule.Enabled = enabled;
                repo.Update(enabledGuildModule);
            }

            await repo.SaveChangesAsync();
            await _cache.RemoveAsync(CacheKey.GetEnabledModulesCacheKey(guildId));
        }
    }
}
