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
        public static IServiceCollection AddCommandEntityFramework<TContext>(this IServiceCollection services)
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
            model.Entity<GuildModule>();
            model.Entity<ChannelPermission>();
            model.Entity<UserPermission>();
            model.Entity<RolePermission>();
            return model;
        }
    }
}
