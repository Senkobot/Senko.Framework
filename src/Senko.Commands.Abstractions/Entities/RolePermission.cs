namespace Senko.Commands.Entities
{
    public class RolePermission : IPermission
    {
        public ulong GuildId { get; set; }

        public ulong RoleId { get; set; }

        public string Name { get; set; }

        public bool Granted { get; set; }
    }
}
