namespace Senko.Commands
{
    public static class CommandBuilderExtensions
    {
        public static ICommandBuilder AddModule<T>(this ICommandBuilder builder)
            => builder.AddModule(typeof(T));
    }
}
