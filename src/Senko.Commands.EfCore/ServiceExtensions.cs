using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senko.Commands.Entities;
using Senko.Commands.Repositories;

namespace Senko.Commands.EfCore
{
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
                builder.HasKey(gm => new { gm.GuildId, gm.Name });
                builder.HasIndex(gm => gm.GuildId);
            });

            model.Entity<ChannelPermission>(builder =>
            {
                builder.HasKey(cp => new { cp.GuildId, cp.ChannelId, cp.Name });
                builder.HasIndex(cp => new { cp.GuildId, cp.ChannelId });
            });

            model.Entity<UserPermission>(builder =>
            {
                builder.HasKey(up => new { up.GuildId, up.UserId, up.Name });
                builder.HasIndex(up => new { up.GuildId, up.UserId });
            });

            model.Entity<RolePermission>(builder =>
            {
                builder.HasKey(rp => new { rp.GuildId, rp.RoleId, rp.Name });
                builder.HasIndex(rp => new { rp.GuildId, rp.RoleId });
            });

            return model;
        }
    }
}
