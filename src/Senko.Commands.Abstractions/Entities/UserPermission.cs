namespace Senko.Commands.Entities
{
    public class UserPermission : IPermission
    {
        public ulong GuildId { get; set; }

        public ulong UserId { get; set; }

        public string Name { get; set; }

        public bool Granted { get; set; }
    }
}
