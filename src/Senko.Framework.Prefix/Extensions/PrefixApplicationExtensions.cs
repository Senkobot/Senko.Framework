using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Senko.Framework.Hosting;
using Senko.Framework.Prefix.Providers;

// ReSharper disable once CheckNamespace
namespace Senko.Framework
{
    public static class PrefixApplicationExtensions
    {
        public const string PrefixItemKey = "Prefix";
        public const string SkipPrefixItemKey = "SkipPrefix";

        /// <summary>
        ///     Get the current prefix.
        ///     This method only works if <see cref="UsePrefix" /> is registered.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The current prefix.</returns>
        public static string GetPrefix(this MessageContext context)
        {
            return context.Items.TryGetValue(PrefixItemKey, out var value) ? (string) value : null;
        }

        public static IBotApplicationBuilder UsePrefix(this IBotApplicationBuilder builder)
        {
            var prefixProvider = builder.ApplicationServices.GetService<IPrefixProvider>();

            return builder.Use(async (context, next) =>
            {
                if (context.Items.TryGetValue(SkipPrefixItemKey, out var skipPrefix) && Equals(skipPrefix, true))
                {
                    await next();
                    return;
                }

                var request = context.Request;
                var mention = context.Self?.Mention;

                string currentPrefix = null;

                if (mention != null && request.Message.StartsWith(mention))
                {
                    request.Message = request.Message.Substring(mention.Length).TrimStart();
                    currentPrefix = mention + " ";
                }
                else if (prefixProvider != null)
                {
                    await foreach (var prefix in prefixProvider.GetPrefixesAsync(context))
                    {
                        if (!request.Message.StartsWith(prefix))
                        {
                            continue;
                        }

                        request.Message = request.Message.Substring(prefix.Length).TrimStart();
                        currentPrefix = prefix;
                        break;
                    }
                }

                if (currentPrefix == null)
                {
                    return;
                }

                context.Items[PrefixItemKey] = currentPrefix;

                await next();
            });
        }
    }
}
