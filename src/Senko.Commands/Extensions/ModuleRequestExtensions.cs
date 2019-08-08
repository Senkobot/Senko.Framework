﻿using System;
using System.Linq;
using Foundatio.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Arguments;
using Senko.Arguments.Abstractions.Exceptions;
using Senko.Commands.Managers;
using Senko.Framework;
using Senko.Framework.Hosting;
using Senko.Localization;

namespace Senko.Commands
{
    public enum CommandError
    {
        /// <summary>
        ///     There was no error and the command has been executed successfully.
        /// </summary>
        None,

        /// <summary>
        ///     The command could not be found.
        /// </summary>
        NotFound,

        /// <summary>
        ///     The guild can only be executed in guilds.
        /// </summary>
        GuildOnly,

        /// <summary>
        ///     The user provided invalid arguments.
        /// </summary>
        InvalidArguments,

        /// <summary>
        ///     The command is currently unavailable due an exception.
        /// </summary>
        Unavailable,

        /// <summary>
        ///     The user tried to execute the command targeting a user, role or channel but there where multiple results found.
        /// </summary>
        AmbiguousArgumentMatch,

        /// <summary>
        ///     The user has no permissions to execute the command.
        /// </summary>
        UserNoPermission,

        /// <summary>
        ///     The server owner disabled the command in the current channel.
        /// </summary>
        ChannelNoPermission,

        /// <summary>
        ///     The module has been disabled.
        /// </summary>
        ModuleDisabled
    }

    public struct PendingCommand
    {
        public PendingCommand(string commandBegin, string commandEnd, ulong[] values, ulong channelId)
        {
            CommandBegin = commandBegin;
            CommandEnd = commandEnd;
            Values = values;
            ChannelId = channelId;
        }

        public ulong ChannelId { get; }

        public string CommandBegin { get; }

        public string CommandEnd { get; }

        public ulong[] Values { get; }
    }

    public static class ModuleRequestExtensions
    {
        private const string BuilderPendingKey = "Senko:SupportsPendinCommands";

        private static string GetPendingCacheKey(ulong userId) => $"senko:pending_commands:{userId}";

        public static IApplicationBuilder UsePendingCommand(this IApplicationBuilder builder)
        {
            builder.Properties[BuilderPendingKey] = true;

            return builder.Use(async (context, next) =>
            {
                if (int.TryParse(context.Request.Message, out var id))
                {
                    id -= 1;

                    var cacheClient = context.RequestServices.GetRequiredService<ICacheClient>();
                    var cache = await cacheClient.GetAsync<PendingCommand>(GetPendingCacheKey(context.User.Id));

                    if (cache.HasValue)
                    {
                        var pendingCommand = cache.Value;

                        if (pendingCommand.ChannelId == context.Request.ChannelId)
                        {
                            if (id >= 0 && id < pendingCommand.Values.Length)
                            {
                                context.Request.Message = ">" + pendingCommand.CommandBegin + pendingCommand.Values[id] + pendingCommand.CommandEnd;
                            }
                            else
                            {
                                context.Response.AddError("You gave an invalid number.");
                                return;
                            }
                        }
                    }
                }

                await next();
            });
        }

        public static IApplicationBuilder UseCommands(this IApplicationBuilder builder)
        {
            var supportsPendingCommands = Equals(builder.Properties[BuilderPendingKey], true);

            return builder.Use(async (context, next) =>
            {
                var commandManager = context.RequestServices.GetRequiredService<ICommandManager>();
                var argumentReader = context.Request.GetArgumentReader();

                argumentReader.Reset();

                var commandName = await argumentReader.ReadStringAsync();
                var commands = commandManager.FindAll(commandName);
                var logger = context.RequestServices.GetRequiredService<ILogger<ICommandManager>>();
                var commandError = CommandError.None;

                if (commands.Count == 0)
                {
                    await next();
                    return;
                }

                var enabledModules = context.Request.GuildId.HasValue
                    ? await context.RequestServices.GetRequiredService<IModuleManager>().GetEnabledModulesAsync(context.Request.GuildId.Value)
                    : null;

                foreach (var command in commands)
                {
                    if (command.GuildOnly && !context.Request.GuildId.HasValue)
                    {
                        commandError = CommandError.GuildOnly;
                        continue;
                    }

                    if (command.Module != null && enabledModules != null && !enabledModules.Contains(command.Module))
                    {
                        commandError = CommandError.ModuleDisabled;
                        continue;
                    }

                    if (!await context.HasChannelPermission(command.Permission))
                    {
                        commandError = CommandError.ChannelNoPermission;
                        continue;
                    }

                    if (!await context.HasUserPermission(command.Permission))
                    {
                        commandError = CommandError.UserNoPermission;
                        continue;
                    }

                    try
                    {
                        await command.ExecuteAsync(context);
                        logger.LogTrace("User {Username} executed the command {CommandName}.", context.User.Username, commandName);
                        commandError = CommandError.None;
                        break;
                    }
                    catch (AmbiguousArgumentMatchException e)
                    {
                        if (!supportsPendingCommands)
                        {
                            context.Response.AddError("There are multiple results found with " + e.Query);
                        }
                        else
                        {
                            var cacheClient = context.RequestServices.GetRequiredService<ICacheClient>();
                            var localizer = context.RequestServices.GetRequiredService<IStringLocalizer>();
                            var key = GetAmbiguousLocalizationKey(e.Type);
                            var items = e.Results.Take(5).ToArray();
                            var ids = items.Select(kv => kv.Key).ToArray();
                            var pendingCommand = new PendingCommand(e.Query.CommandBegin, e.Query.CommandEnd, ids, context.Request.ChannelId);

                            context.Response.AddError(
                                localizer[key]
                                    .WithToken("Results", "\n" + string.Join("\n", items.Select((kv, i) => $"{i + 1}: `{kv.Value}`")))
                                    .WithToken("Query", e.Query.Value)
                            );

                            await cacheClient.SetAsync(GetPendingCacheKey(context.User.Id), pendingCommand, TimeSpan.FromMinutes(1));

                            commandError = CommandError.AmbiguousArgumentMatch;
                        }
                        break;
                    }
                    catch (MissingArgumentException)
                    {
                        commandError = CommandError.InvalidArguments;
                    }
                    catch (CommandNotAvailableException)
                    {
                        logger.LogTrace("User {Username} could not execute the command {CommandName}.", context.User.Username, commandName);
                        commandError = CommandError.Unavailable;
                    }
                }

                switch (commandError)
                {
                    case CommandError.None:
                    case CommandError.NotFound:
                    case CommandError.ModuleDisabled:
                        // ignore.
                        break;
                    case CommandError.AmbiguousArgumentMatch:
                        // ignore, a message was already sent with more information.
                        break;
                    case CommandError.GuildOnly:
                        context.Response.AddError("Whoops. This command can only be executed in a guild.");
                        logger.LogDebug("User {Username} tried to execute the command {CommandName} but the command can only be executed in guild channels.", context.User.Username, commandName);
                        break;
                    case CommandError.ChannelNoPermission:
                        logger.LogDebug("User {Username} tried to execute the command {CommandName} but the server owner disabled the permission in the channel.", context.User.Username, commandName);
                        break;
                    case CommandError.UserNoPermission:
                        context.Response.AddError("Whoops. You don't have permissions to do that.");
                        logger.LogDebug("User {Username} tried to execute the command {CommandName} but didn't have the required permissions to do so.", context.User.Username, commandName);
                        break;
                    case CommandError.InvalidArguments:
                        context.Response.AddError("Invalid arguments."); // TODO: More information
                        logger.LogTrace("User {Username} tried to execute the command {CommandName} but didn't provide all the arguments.", context.User.Username, commandName);
                        break;
                    case CommandError.Unavailable:
                        context.Response.AddMessage("The command is currently not available.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await next();
            });
        }

        private static string GetAmbiguousLocalizationKey(ArgumentType type)
        {
            switch (type)
            {
                case ArgumentType.UserMention:
                    return "Command.AmbiguousUser";
                case ArgumentType.RoleMention:
                    return "Command.AmbiguousRole";
                case ArgumentType.Channel:
                    return "Command.AmbiguousChannel";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
