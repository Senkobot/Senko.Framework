using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Senko.AspNetCore.Example.Events;
using Senko.AspNetCore.Example.Hubs;
using Senko.Events;
using Senko.Framework;
using Senko.Framework.Hosting;

namespace Senko.AspNetCore.Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEventListener<BotEvents>();
            services.AddSignalR();
            services.AddRazorPages();
        }

        public void ConfigureBot(IBotApplicationBuilder app)
        {
            app.UseIgnoreBots();
            app.Use((context, next) =>
            {
                if (context.Request.Message == "ping")
                {
                    context.Response.AddMessage("Pong");
                }

                return next();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<BotHub>("/botHub");
            });
        }
    }
}
