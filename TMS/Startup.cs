using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.Authentication.Cookies;
using TMS.Services;

namespace TMS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services">Services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataSorter<QTask>, TaskSorter>();
            services.AddScoped<ITMSRepository, TMSRepository>();
            services.AddScoped<IRepositoryHandler<QTask>, TaskHandler>();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddDbContext<TMSRepository>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromDays(3);
                        options.SlidingExpiration = true;
                        options.LoginPath = new PathString("/Account/Login");
                    });
            services.AddControllersWithViews();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                     name: "default",
                     template: "{controller}/{action}/{id?}",
                     defaults: new
                     {
                         controller = "Home",
                         action = "Index"
                     },
                     constraints: new
                     {
                         id = new RegexRouteConstraint("\\d+")
                     }
                );
            });
        }
    }
}
