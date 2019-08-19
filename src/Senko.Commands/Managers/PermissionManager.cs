using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Discord;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Events;
using CacheKey = Senko.Common.CacheKey;

namespace Senko.Commands.Managers
{
    public class PermissionManager : IPermissionManager, IEventListener
    {
        private Dictionary<PermissionGroup, List<IPermission>> _defaultPermissions;
        private readonly ILogger<PermissionManager> _logger;
        private readonly IServiceProvider _provider;
        private readonly ICacheClient _cache;
        private readonly IDiscordClient _client;
        private readonly PermissionOptions _options;

        public PermissionManager(IServiceProvider provider, ICacheClient cache, IDiscordClient client, ILogger<PermissionManager> logger, IOptions<PermissionOptions> options)
        {
            _provider = provider;
            _cache = cache;
            _client = client;
            _logger = logger;
            _options = options.Value;
        }

        [EventListener(typeof(InitializeEvent), EventPriority.High, PriorityOrder = 200)]
        public virtual Task InitializeAsync()
        {
            var commandManager = _provider.GetRequiredService<ICommandManager>();

            Permissions = commandManager.Commands.Select(c => c.Permission).Distinct().ToList();
            _defaultPermissions = commandManager.Commands
                .GroupBy(c => c.PermissionGroup)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(c => new Permission(c.Permission, true)).Cast<IPermission>().ToList()
                );

            return Task.CompletedTask;
        }

        [EventListener(EventPriority.High)]
        public Task OnMemberRolesUpdatedEvent(GuildMemberRolesUpdateEvent e)
        {
            return _cache.RemoveAsync(CacheKey.GetUserPermissionCacheKey(e.Member.GuildId, e.Member.Id));
        }

        public static TimeSpan CacheTime { get; set; } = TimeSpan.FromMinutes(5);

        public IReadOnlyList<string> Permissions { get; private set; } = Array.Empty<string>();

        public async Task<IReadOnlyList<PermissionGroup>> GetPermissionGroups(ulong userId, ulong? guildId)
        {
            if (!guildId.HasValue)
            {
                return GetDefaultPermissions(await _client.GetUserAsync(userId));
            }

            var user = await _client.GetGuildUserAsync(userId, guildId.Value);
            var guild = await _client.GetGuildAsync(guildId.Value);

            return await GetPermissionGroups(user, guild);
        }

        private List<PermissionGroup> GetDefaultPermissions(IDiscordUser user)
        {
            var groups = new List<PermissionGroup>
            {
                PermissionGroup.User
            };

            if (_options.Developers.Contains(user.Id))
            {
                groups.Add(PermissionGroup.Developer);
            }

            return groups;
        }

        private async Task<List<PermissionGroup>> GetPermissionGroups(IDiscordGuildUser user, IDiscordGuild guild)
        {
            var userPermissions = await guild.GetPermissionsAsync(user);
            var groups = GetDefaultPermissions(user);

            if (userPermissions.HasFlag(_options.ModeratorPermission))
            {
                groups.Add(PermissionGroup.Moderator);
            }

            if (userPermissions.HasFlag(_options.AdministratorPermission))
            {
                groups.Add(PermissionGroup.Administrator);
            }

            return groups;
        }

        public async Task<IReadOnlyCollection<string>> GetAllowedUserPermissionAsync(ulong userId, ulong? guildId)
        {
            var cacheKey = CacheKey.GetUserPermissionCacheKey(guildId, userId);
            var cache = await _cache.GetAsync<string[]>(cacheKey);

            if (cache.HasValue)
            {
                return cache.Value;
            }

            var stopwatch = Stopwatch.StartNew();

            using var scope = _provider.CreateScope();
            var roleRepo = scope.ServiceProvider.GetService<IRolePermissionRepository>();
            var userRepo = scope.ServiceProvider.GetService<IUserPermissionRepository>();

            IDiscordUser user;
            IReadOnlyList<PermissionGroup> userPermissions;
            IReadOnlyList<IDiscordRole> userRoles;

            if (guildId.HasValue)
            {
                var guildUser = await _client.GetGuildUserAsync(userId, guildId.Value);
                var guild = await _client.GetGuildAsync(guildId.Value);
                var roles = await guild.GetRolesAsync();
                userRoles = roles.Where(r => guildUser.RoleIds.Contains(r.Id)).ToList();

                user = guildUser;
                userPermissions = await GetPermissionGroups(guildUser, guild);
            }
            else
            {
                userRoles = new IDiscordRole[0];
                user = await _client.GetUserAsync(userId);
                userPermissions = GetDefaultPermissions(user);
            }

            var permissions = new Dictionary<string, bool>();

            void AddPermissions(IEnumerable<IPermission> newPermissions)
            {
                foreach (var permission in newPermissions)
                {
                    if (!permissions.ContainsKey(permission.Name))
                    {
                        permissions[permission.Name] = permission.Granted;
                    }
                }
            }

            // Add the default permissions.
            if (userPermissions.Contains(PermissionGroup.Developer)
                && _defaultPermissions.TryGetValue(PermissionGroup.Developer, out var newPermissions))
            {
                AddPermissions(newPermissions);
            }

            if (userPermissions.Contains(PermissionGroup.Administrator)
                && _defaultPermissions.TryGetValue(PermissionGroup.Administrator, out newPermissions))
            {
                AddPermissions(newPermissions);
            }

            // Add the user permissions from the database.
            if (userRepo != null && guildId.HasValue)
            {
                AddPermissions(await userRepo.GetAllAsync(guildId.Value, userId));
            }

            // Add the role permissions from the database.
            if (roleRepo != null && guildId.HasValue)
            {
                foreach (var role in userRoles.OrderBy(r => r.Id))
                {
                    AddPermissions(await roleRepo.GetAllAsync(guildId.Value, role.Id));
                }
            }

            // Add the default permissions.
            if (userPermissions.Contains(PermissionGroup.Moderator)
                && _defaultPermissions.TryGetValue(PermissionGroup.Moderator, out newPermissions))
            {
                AddPermissions(newPermissions);
            }

            // Add the everyone role
            if (roleRepo != null && guildId.HasValue)
            {
                AddPermissions(await roleRepo.GetAllAsync(guildId.Value, guildId.Value));
            }

            if (_defaultPermissions.TryGetValue(PermissionGroup.User, out newPermissions))
            {
                AddPermissions(newPermissions);
            }

            // Store into the cache.
            var allowedPermissions = permissions.Where(kv => kv.Value).Select(kv => kv.Key).ToArray();

            _logger.LogDebug("Loaded the permissions for {Username} in {Duration:0.00} ms.", user.Username, stopwatch.Elapsed.TotalMilliseconds);

            await _cache.SetAsync(cacheKey, allowedPermissions, CacheTime);

            return allowedPermissions;
        }

        public async Task<IReadOnlyCollection<string>> GetAllowedChannelPermissionAsync(ulong channelId, ulong? guildId)
        {
            if (!guildId.HasValue)
            {
                return Permissions;
            }

            var cacheKey = CacheKey.GetChannelPermissionCacheKey(guildId, channelId);
            var cache = await _cache.GetAsync<string[]>(cacheKey);

            if (cache.HasValue)
            {
                return cache.Value;
            }

            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetService<IChannelPermissionRepository>();
            string[] allowedPermissions;

            if (repo == null)
            {
                allowedPermissions = Permissions.ToArray();
            } 
            else
            {
                var disallowedPermissions = (await repo.GetAllAsync(guildId.Value, channelId))
                    .Where(p => !p.Granted)
                    .Select(p => p.Name)
                    .ToList();

                allowedPermissions = Permissions
                    .Where(p => !disallowedPermissions.Contains(p))
                    .ToArray();
            }

            await _cache.SetAsync(cacheKey, allowedPermissions, CacheTime);

            return allowedPermissions;
        }

        public async Task<bool> HasUserPermissionAsync(ulong userId, string permission, ulong? guildId = null)
        {
            var allowedPermissions = await GetAllowedUserPermissionAsync(userId, guildId);

            return allowedPermissions.Contains(permission);
        }

        public async Task<bool> HasChannelPermissionAsync(ulong channelId, string permission, ulong? guildId)
        {
            if (!guildId.HasValue)
            {
                return true;
            }

            var allowedPermissions = await GetAllowedChannelPermissionAsync(channelId, guildId);

            return allowedPermissions.Contains(permission);
        }

        public async Task<bool> SetUserPermissionAsync(ulong guildId, ulong userId, string permissionName, bool? granted)
        {
            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IUserPermissionRepository>();
            var permission = await repo.GetAsync(guildId, userId, permissionName);

            if (permission == null)
            {
                if (!granted.HasValue)
                {
                    return true;
                }

                permission = new UserPermission
                {
                    GuildId = guildId,
                    UserId = userId,
                    Name = permissionName,
                    Granted = granted.Value
                };

                repo.Add(permission);
                await repo.SaveChangesAsync();
            }
            else if (!granted.HasValue || permission.Granted != granted)
            {
                if (granted.HasValue)
                {
                    permission.Granted = granted.Value;

                    repo.Update(permission);
                }
                else
                {
                    repo.Remove(permission);
                }

                await repo.SaveChangesAsync();
            }
            else
            {
                return false;
            }

            await _cache.RemoveAsync(CacheKey.GetUserPermissionCacheKey(guildId, userId));
            return true;
        }

        public async Task<bool> SetRolePermissionAsync(ulong guildId, ulong roleId, string permissionName, bool? granted)
        {
            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRolePermissionRepository>();
            var permission = await repo.GetAsync(guildId, roleId, permissionName);

            if (permission == null)
            {
                if (!granted.HasValue)
                {
                    return true;
                }

                permission = new RolePermission
                {
                    GuildId = guildId,
                    RoleId = roleId,
                    Name = permissionName,
                    Granted = granted.Value
                };

                repo.Add(permission);
                await repo.SaveChangesAsync();
            }
            else if (!granted.HasValue || permission.Granted != granted)
            {
                if (granted.HasValue)
                {
                    permission.Granted = granted.Value;

                    repo.Update(permission);
                }
                else
                {
                    repo.Remove(permission);
                }

                await repo.SaveChangesAsync();
            }
            else
            {
                return false;
            }

            var guild = await _client.GetGuildAsync(guildId);
            var members = await guild.GetMembersAsync();

            
            IEnumerable<string> cacheKeys;

            if (roleId == guildId)
            {
                cacheKeys = members.Select(user => CacheKey.GetUserPermissionCacheKey(guildId, user.Id));
            }
            else
            {
                cacheKeys = members
                    .Where(user => user.RoleIds.Contains(roleId))
                    .Select(user => CacheKey.GetUserPermissionCacheKey(guildId, user.Id));
            }

            await _cache.RemoveAllAsync(cacheKeys);

            return true;
        }

        public async Task<bool> SetChannelPermissionAsync(ulong guildId, ulong channelId, string permissionName, bool? granted)
        {
            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IChannelPermissionRepository>();
            var permission = await repo.GetAsync(guildId, channelId, permissionName);

            if (permission == null)
            {
                if (!granted.HasValue)
                {
                    return true;
                }

                permission = new ChannelPermission
                {
                    GuildId = guildId,
                    ChannelId = channelId,
                    Name = permissionName,
                    Granted = granted.Value
                };

                repo.Add(permission);
                await repo.SaveChangesAsync();
            }
            else if (!granted.HasValue || permission.Granted != granted)
            {
                if (granted.HasValue)
                {
                    permission.Granted = granted.Value;

                    repo.Update(permission);
                }
                else
                {
                    repo.Remove(permission);
                }

                await repo.SaveChangesAsync();
            }
            else
            {
                return false;
            }

            await _cache.RemoveAsync(CacheKey.GetChannelPermissionCacheKey(guildId, channelId));

            return true;
        }
    }
}
