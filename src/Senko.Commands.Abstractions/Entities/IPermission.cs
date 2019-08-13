namespace Senko.Commands.Entities
{
    public interface IPermission
    {
        string Name { get; }

        bool Granted { get; }
    }
}
