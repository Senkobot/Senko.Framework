namespace Senko.Commands.Entities
{
    public class Permission : IPermission
    {
        public Permission(string permission, bool granted)
        {
            Name = permission;
            Granted = granted;
        }

        public string Name { get; }

        public bool Granted { get; }
    }
}
