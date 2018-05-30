using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Odonto.WebApp.Helpers.Filters;

namespace Odonto.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddSessionStateTempDataProvider();

            services.AddSession();

            // Auth Primitives
            services.AddScoped<IsLoggedAttribute>();
            services.AddScoped<IsNotLoggedAttribute>();
            services.AddScoped<CheckAccessAttribute>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseExceptionHandler("/Error/Exception");
            }
            else
            {
                app.UseExceptionHandler("/Error/Exception");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
