using System;
using System.Threading.Tasks;

namespace Senko.Framework.Hosting
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds a middleware delegate defined in-line to the application's request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IBotApplicationBuilder"/> instance.</param>
        /// <param name="middleware">A function that handles the request or calls the given next function.</param>
        /// <returns>The <see cref="IBotApplicationBuilder"/> instance.</returns>
        public static IBotApplicationBuilder Use(this IBotApplicationBuilder app, Func<MessageContext, Func<ValueTask>, ValueTask> middleware)
        {
            return app.Use(next =>
            {
                return context =>
                {
                    return middleware(context, () => next(context));
                };
            });
        }
    }
}
