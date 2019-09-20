using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Senko.Commands;
using Senko.Commands.Managers;
using Senko.Events;
using Senko.Framework.Managers;

namespace Senko.Framework
{
    public static class GuildOptionsExtensions
    {
        public static IServiceCollection AddGuildOptions(this IServiceCollection services)
        {
            services.AddSingleton<IGuildOptionsManager, GuildOptionsManager>();
            services.AddSingleton<ICommandValueProvider, GuildOptionsCommandValueProvider>();

            return services;
        }
    }
}
