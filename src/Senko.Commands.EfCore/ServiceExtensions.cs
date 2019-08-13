using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;
using Senko.Framework.Options;

namespace Senko.Commands.EfCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCommandEfCoreRepositories<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IGuildModuleRepository, GuildModuleRepository<TContext>>();
            services.AddScoped<IChannelPermissionRepository, ChannelPermissionRepository<TContext>>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository<TContext>>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository<TContext>>();
            return services;
        }

        public static ModelBuilder AddCommand(this ModelBuilder model)
        {
            model.Entity<GuildModule>(builder =>
            {
                builder.HasKey(gm => gm.GuildId);
                builder.HasAlternateKey(gm => new { gm.GuildId, gm.Name });
            });

            model.Entity<ChannelPermission>(builder =>
            {
                builder.HasKey(cp => new { cp.GuildId, cp.ChannelId });
                builder.HasAlternateKey(cp => new { cp.GuildId, cp.ChannelId, cp.Name });
            });

            model.Entity<UserPermission>(builder =>
            {
                builder.HasKey(up => new { up.GuildId, up.UserId });
                builder.HasAlternateKey(up => new { up.GuildId, up.UserId, up.Name });
            });

            model.Entity<RolePermission>(builder =>
            {
                builder.HasKey(rp => new { rp.GuildId, rp.RoleId });
                builder.HasAlternateKey(rp => new {rp.GuildId, rp.RoleId, rp.Name});
            });

            return model;
        }
    }
}
