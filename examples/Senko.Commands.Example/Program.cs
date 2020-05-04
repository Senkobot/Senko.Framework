using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Senko.Arguments;
using Senko.Framework;
using Senko.Framework.Hosting;
using Senko.Framework.Repositories;
using Senko.Framework.Repository;
using Senko.Localization;

namespace Senko.Commands.Example
{
    internal class Program
    {
        public static Task Main(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddArgumentWithParsers();
                    services.AddLocalizations();

                    services.AddCommand(builder =>
                    {
                        builder.AddModules(typeof(Program).Assembly);
                    });

                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                    });

                    // Part of Senko.Framework.Prefix
                    services.AddPrefix(">");

                    // Part of Senko.Framework.GuildOptions
                    services.AddGuildOptions<MemoryGuildOptionRepository>();
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddEnvironmentVariables();
                })
                .ConfigureDiscordBot(builder =>
                {
                    builder.UseIgnoreBots();
                    builder.UsePendingCommand();
                    builder.UsePrefix();
                    builder.UseCommands();
                })
                .Build()
                .RunAsync();
        }
    }
}
