using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Arguments;
using Senko.Framework;
using Senko.Framework.Hosting;
using Senko.Localization;

namespace Senko.Commands.Example
{
    internal class Program
    {
        public static Task Main(string[] args)
        {
            return new BotHostBuilder()
                .ConfigureService(services =>
                {
                    services.AddArgumentWithParsers();
                    services.AddLocalizations();
                    services.AddCommand();
                    services.AddModules(typeof(Program).Assembly);
                    
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
                    builder.UsePendingCommand();
                    builder.UsePrefix(">");
                    builder.UseCommands();
                })
                .Build()
                .RunAsync();
        }
    }
}
