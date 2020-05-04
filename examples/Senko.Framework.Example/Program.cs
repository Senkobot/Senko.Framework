using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Senko.Framework.Hosting;

namespace Senko.Framework.Example
{
    internal class Program
    {
        public static Task Main(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddPrefix(">");

                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                    });
                })
                .ConfigureDiscordBot(builder =>
                {
                    builder.UseIgnoreBots();
                    builder.UsePrefix();
                    builder.Use((context, next) =>
                    {
                        if (context.Request.Message == "ping")
                        {
                            context.Response.AddMessage("Pong");
                        }
                        
                        return next();
                    });
                })
                .Build()
                .RunAsync();
        }
    }
}
