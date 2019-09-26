﻿using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Arguments;
using Senko.Commands.Example.Repository;
using Senko.Framework;
using Senko.Framework.Hosting;
using Senko.Framework.Repositories;
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
                    services.AddCommand()
                        .AddModules(typeof(Program).Assembly);

                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                    });

                    // Part of Senko.Framework.Prefix
                    services.AddPrefix(">");

                    // Part of Senko.Framework.GuildOptions
                    services.AddGuildOptions();
                    services.AddScoped<IGuildOptionRepository, MemoryGuildOptionRepository>();
                })
                .ConfigureOptions(builder =>
                {
                    builder.AddEnvironmentVariables();
                })
                .Configure(builder =>
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
