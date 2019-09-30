using System;
using System.Threading.Tasks;
using Senko.Framework.Hosting;

namespace Senko.Framework
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseIgnoreBots(this IApplicationBuilder builder)
        {
            return builder.Use((context, next) => context.User.IsBot ? default: next());
        }
    }
}
