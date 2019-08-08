using System.Threading.Tasks;
using Senko.Framework.Hosting;

namespace Senko.Framework
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseIgnoreBots(this IApplicationBuilder builder)
        {
            return builder.Use((context, next) => context.User.IsBot ? Task.CompletedTask : next());
        }

        public static IApplicationBuilder UsePrefix(this IApplicationBuilder builder, params string[] prefixes)
        {
            return builder.Use((context, next) =>
            {
                var request = context.Request;

                foreach (var prefix in prefixes)
                {
                    if (!request.Message.StartsWith(prefix))
                    {
                        continue;
                    }

                    request.Message = request.Message.Substring(prefix.Length).TrimStart();
                    return next();
                }

                return Task.CompletedTask;
            });
        }
    }
}
