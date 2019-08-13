using System;

namespace Senko.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string id, PermissionGroup permissionGroup = PermissionGroup.User)
        {
            Id = id;
            PermissionGroup = permissionGroup;
        }

        public string Id { get; }

        public PermissionGroup PermissionGroup { get; set; }

        public bool GuildOnly { get; set; }
    }
}
