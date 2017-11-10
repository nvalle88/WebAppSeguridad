using bd.webappcompartido.servicios.Servicios;
using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.servicios.Servicios;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;

namespace bd.webappcompartido.web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public async void ConfigureServices(IServiceCollection services)
        {

            // Add framework services.
            services.AddMvc();
            services.AddSingleton<IBaseDatosServicio, BaseDatosServicio>();
            services.AddSingleton<IAdscSistServicio, AdscSistServicio>();
            services.AddSingleton<IApiServicio, ApiServicio>();
            services.AddResponseCaching();
            
            services.AddSingleton<IAdscpasswServicio, AdscpasswServicio>();

            var ServicioSeguridad = Configuration.GetSection("ServicioSeguridad").Value;
            var ServiciosLog = Configuration.GetSection("ServiciosLog").Value;
            var HostSeguridad = Configuration.GetSection("HostServicioSeguridad").Value;
            WebApp.NombreAplicacionSeguridad = Configuration.GetSection("NombreAplicacionSeguridad").Value;
            WebApp.NombreAplicacionLog = Configuration.GetSection("NombreAplicacionSeguridad").Value;

            //await InicializarWebApp.InicializarWeb("SeguridadWebService", new Uri("http://192.168.100.21:8081"));
            //await InicializarWebApp.InicializarLogEntry("LogWebService", new Uri("http://192.168.100.21:8081"));
            await InicializarWebApp.InicializarWeb(ServicioSeguridad, new Uri(HostSeguridad));
            await InicializarWebApp.InicializarLogEntry(ServiciosLog, new Uri(HostSeguridad));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();




            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();


                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
                {
                    //serviceScope.ServiceProvider.GetService<LogDbContext>()
                    //         .Database.Migrate();

                   // serviceScope.ServiceProvider.GetService<InicializacionServico>().InicializacionAsync();
                }

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

           


            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            app.UseResponseCaching();

            app.Run(async (context) =>
            {
                context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    
                };
            });
        }
    }
}
