using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuchShop.Geschaeftslogik.Geschaeftsservices;
using BuchShop.Datenzugriff;
using BuchShop.Services;

namespace BuchShop
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

            // service Registration
            // services.AddSingleton<IBestellservice, Bestellservice>();
            //services.AddScoped<INutzerservice, Nutzerservice>();
     
            // Add framework services. 
            services.AddMvc();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddSingleton<INutzerservice, Nutzerservice>();
            services.AddSingleton<IBestellservice, Bestellservice>();
            services.AddSingleton<IDatenbankZugriff, DatenbankZugriffFake>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=BuchShop}/{action=Index}/{id?}");
            });
        }

    }
}
