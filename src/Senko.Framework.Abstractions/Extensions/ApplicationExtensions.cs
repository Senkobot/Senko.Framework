using Senko.Framework.Hosting;

namespace Senko.Framework
{
    public static class ApplicationExtensions
    {
        public static IBotApplicationBuilder UseIgnoreBots(this IBotApplicationBuilder builder)
        {
            return builder.Use((context, next) => context.User.IsBot ? default: next());
        }
    }
}
