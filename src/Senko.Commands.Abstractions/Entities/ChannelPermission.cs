namespace Senko.Commands.Entities
{
    public class ChannelPermission : IPermission
    {
        public ulong GuildId { get; set; }

        public ulong ChannelId { get; set; }

        public string Name { get; set; }

        public bool Granted { get; set; }
    }
}
