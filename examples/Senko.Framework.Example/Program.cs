using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Framework.Hosting;

namespace Senko.Framework.Example
{
    internal class Program
    {
        public static Task Main(string[] args)
        {
            return new BotHostBuilder()
                .ConfigureService(services =>
                {
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                    });
                })
                .ConfigureOptions(builder =>
                {
                    builder.AddEnvironmentVariables();
                })
                .Configure(builder =>
                {
                    builder.UseIgnoreBots();
                    builder.UsePrefix(">");
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
