using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senko.Commands.Managers;
using Senko.Discord;
using Senko.Framework;
using Senko.Localization;

namespace Senko.Commands.Modules
{
    [CoreModule]
    public class PermissionModule : ModuleBase
    {
        private readonly IPermissionManager _permissionManager;
        private readonly IStringLocalizer _localizer;
        private readonly IModuleManager _moduleManager;
        private readonly IDiscordGuild _guild;

        public PermissionModule(IPermissionManager permissionManager, IStringLocalizer localizer, IModuleManager moduleManager, IDiscordGuild guild)
        {
            _permissionManager = permissionManager;
            _localizer = localizer;
            _moduleManager = moduleManager;
            _guild = guild;
        }

        private bool ValidatePermission(string permission)
        {
            if (!_permissionManager.Permissions.Contains(permission))
            {
                Response.AddError(_localizer["Command.PermissionNotFound"].WithToken("Name", permission));
                return false;
            }

            if (permission.StartsWith("permission.", StringComparison.OrdinalIgnoreCase))
            {
                Response.AddError(_localizer["Command.PermissionBlocked"].WithToken("Name", permission));
                return false;
            }

            return true;
        }

        [Command("permissions", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task PermissionsAsync(IDiscordGuildUser user)
        {
            var allowedPermissions = await _permissionManager.GetAllowedUserPermissionAsync(user.Id, _guild.Id);

            await PermissionsAsync("User", user, allowedPermissions);
        }

        [Command("permissions", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task PermissionsAsync(IDiscordChannel channel)
        {
            var allowedPermissions = await _permissionManager.GetAllowedChannelPermissionAsync(channel.Id, _guild.Id);

            await PermissionsAsync("Channel", channel, allowedPermissions);
        }

        [Command("permissions", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task PermissionsAsync(IDiscordRole role)
        {
            var allowedPermissions = await _permissionManager.GetAllowedRolePermissionAsync(role.Id, _guild.Id);

            await PermissionsAsync("Role", role, allowedPermissions);
        }

        [Command("permissions", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task PermissionsAsync(IDiscordGuild guild)
        {
            var user = Context.User;
            var allowedPermissions = await _permissionManager.GetAllowedUserPermissionAsync(user.Id, _guild.Id);

            await PermissionsAsync("User", user, allowedPermissions);
        }

        private async Task PermissionsAsync(
            string type,
            object target,
            IReadOnlyCollection<string> allowedPermissions)
        {
            var guildId = _guild.Id;
            var permissions = _permissionManager.Permissions;
            var allowedChannelPermissions = !(target is IDiscordChannel)
                ? await _permissionManager.GetAllowedChannelPermissionAsync(Context.Request.ChannelId, guildId)
                : null;
            var content = GetString("Command.Permissions.Message", type, null, target);
            var message = Response.AddMessage(content);
            var enabledModules = await _moduleManager.GetEnabledModulesAsync(guildId);
            var modules = permissions
                .GroupBy(p => p.Substring(0, p.IndexOf('.')))
                .Where(g => enabledModules.Contains(g.Key, StringComparer.OrdinalIgnoreCase))
                .OrderBy(g => g.Count())
                .ToArray();

            string GetStatusEmoji(string p)
            {
                if (!allowedPermissions.Contains(p))
                {
                    return _localizer["Emoji.Permissions.Denied"];
                }

                if (allowedChannelPermissions != null && !allowedChannelPermissions.Contains(p))
                {
                    return _localizer["Emoji.Permissions.ChannelDenied"];
                }

                return _localizer["Emoji.Permissions.Allowed"];
            }

            foreach (var group in modules)
            {
                var items = group.Select(p => GetStatusEmoji(p) + " " + p.Substring(p.IndexOf('.') + 1));

                message.AddEmbedField(group.Key, string.Join('\n', items), true);
            }

            var remaining = modules.Length % 3;

            for (var i = remaining; i >= 0; i--)
            {
                message.AddEmbedField("‌‌ ", "‌‌ ", true);
            }
        }

        [Command("grant", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task GrantAsync(IDiscordGuildUser user, string permission)
        {
            if (!ValidatePermission(permission))
            {
                return;
            }

            if (!await _permissionManager.GrantUserPermissionAsync(user.GuildId, user.Id, permission))
            {
                Response.AddError(GetString("Command.Grant.Failed", "User", permission, user));
            }
            else
            {
                Response.AddSuccess(GetString("Command.Grant.Success", "User", permission, user));
            }
        }

        [Command("grant", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task GrantAsync(IDiscordRole role, string permission)
        {
            if (!ValidatePermission(permission))
            {
                return;
            }

            if (!await _permissionManager.GrantRolePermissionAsync(_guild.Id, role.Id, permission))
            {
                Response.AddError(GetString("Command.Grant.Failed", "Role", permission, role));
            }
            else
            {
                Response.AddSuccess(GetString("Command.Grant.Success", "Role", permission, role));
            }
        }

        [Command("grant", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task GrantAsync(IDiscordChannel channel, string permission)
        {
            if (!ValidatePermission(permission))
            {
                return;
            }

            if (!await _permissionManager.GrantChannelPermissionAsync(_guild.Id, channel.Id, permission))
            {
                Response.AddError(GetString("Command.Grant.Failed", "Channel", permission, channel));
            }
            else
            {
                Response.AddSuccess(GetString("Command.Grant.Success", "Channel", permission, channel));
            }
        }

        [Command("revoke", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task RevokeAsync(IDiscordGuildUser user, string permission)
        {
            if (!ValidatePermission(permission))
            {
                return;
            }

            if (!await _permissionManager.RevokeUserPermissionAsync(_guild.Id, user.Id, permission))
            {
                Response.AddError(GetString("Command.Revoke.Failed", "User", permission, user));
            }
            else
            {
                Response.AddSuccess(GetString("Command.Revoke.Success", "User", permission, user));
            }
        }

        [Command("revoke", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task RevokeAsync(IDiscordRole role, string permission)
        {
            if (!ValidatePermission(permission))
            {
                return;
            }

            if (!await _permissionManager.RevokeRolePermissionAsync(_guild.Id, role.Id, permission))
            {
                Response.AddError(GetString("Command.Revoke.Failed", "Role", permission, role));
            }
            else
            {
                Response.AddSuccess(GetString("Command.Revoke.Success", "Role", permission, role));
            }
        }

        [Command("revoke", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task RevokeAsync(IDiscordChannel channel, string permission)
        {
            if (!ValidatePermission(permission))
            {
                return;
            }

            if (!await _permissionManager.RevokeChannelPermissionAsync(_guild.Id, channel.Id, permission))
            {
                Response.AddError(GetString("Command.Revoke.Failed", "Channel", permission, channel));
            }
            else
            {
                Response.AddSuccess(GetString("Command.Revoke.Success", "Channel", permission, channel));
            }
        }

        [Command("reset", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task ResetAsync(IDiscordGuildUser user, string permission)
        {
            if (!ValidatePermission(permission))
            {
                return;
            }

            if (!await _permissionManager.ResetUserPermissionAsync(_guild.Id, user.Id, permission))
            {
                Response.AddError(GetString("Command.Reset.Failed", "User", permission, user));
            }
            else
            {
                Response.AddSuccess(GetString("Command.Reset.Success", "User", permission, user));
            }
        }

        [Command("reset", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task ResetAsync(IDiscordRole role, string permission)
        {
            if (!ValidatePermission(permission))
            {
                return;
            }

            if (!await _permissionManager.ResetRolePermissionAsync(_guild.Id, role.Id, permission))
            {
                Response.AddError(GetString("Command.Reset.Failed", "Role", permission, role));
            }
            else
            {
                Response.AddSuccess(GetString("Command.Reset.Success", "Role", permission, role));
            }
        }

        [Command("reset", PermissionGroup.Administrator, GuildOnly = true)]
        public async Task ResetAsync(IDiscordChannel channel, string permission)
        {
            if (!ValidatePermission(permission))
            {
                return;
            }

            if (!await _permissionManager.ResetChannelPermissionAsync(_guild.Id, channel.Id, permission))
            {
                Response.AddError(GetString("Command.Reset.Failed", "Channel", permission, channel));
            }
            else
            {
                Response.AddSuccess(GetString("Command.Reset.Success", "Channel", permission, channel));
            }
        }

        private string GetString(string name, string type, string permission, object target)
        {
            var localized = _localizer[name]
                .WithToken("Target", target)
                .WithToken("Type", _localizer["Type." + type].ToString().ToLower())
                .WithToken("Name", '`' + permission + '`')
                .ToString();

            return char.ToUpper(localized[0]) + localized.Substring(1);
        }
    }
}
