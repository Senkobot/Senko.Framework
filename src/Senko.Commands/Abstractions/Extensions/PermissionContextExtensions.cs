using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Commands.Managers;
using Senko.Framework;

// ReSharper disable once CheckNamespace
namespace Senko.Commands
{
    public static class PermissionContextExtensions
    {
        public static Task<bool> HasUserPermission(this MessageContext context, string permission)
        {
            var permissionManager = context.RequestServices.GetRequiredService<IPermissionManager>();
            var guildId = context.Request.GuildId;

            return permissionManager.HasUserPermissionAsync(context.User.Id, permission, guildId);
        }

        public static Task<bool> HasChannelPermission(this MessageContext context, string permission)
        {
            var permissionManager = context.RequestServices.GetRequiredService<IPermissionManager>();

            return permissionManager.HasChannelPermissionAsync(context.Request.ChannelId, permission, context.Request.GuildId);
        }
    }
}
