using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.servicios.Servicios;
using bd.webappseguridad.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;

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
            services.AddSingleton<IAdscSistServicio, AdscSistServicio>();
            services.AddSingleton<IApiServicio, ApiServicio>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EstaAutorizado",
                                  policy => policy.Requirements.Add(new RolesRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, RolesHandler>();

            services.AddResponseCaching();
            
            services.AddSingleton<IAdscpasswServicio, AdscpasswServicio>();

            var HostSeguridad = Configuration.GetSection("HostServicioSeguridad").Value;
            WebApp.BaseAddressWebAppLogin= Configuration.GetSection("HostLogin").Value;
            WebApp.NombreAplicacion = Configuration.GetSection("NombreAplicacion").Value;

            await InicializarWebApp.InicializarWeb(HostSeguridad);
            await InicializarWebApp.InicializarLogEntry(Configuration.GetSection("ServiciosLog").Value, new Uri(HostSeguridad));


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

            var TiempoVidaCookieHoras = Configuration.GetSection("TiempoVidaCookieHoras").Value;
            var TiempoVidaCookieMinutos = Configuration.GetSection("TiempoVidaCookieMinutos").Value;
            var TiempoVidaCookieSegundos = Configuration.GetSection("TiempoVidaCookieSegundos").Value;

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                LoginPath = new PathString("/"),
                AccessDeniedPath = new PathString("/Home/AccesoDenegado"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                CookieName = "ASPTest",
                ExpireTimeSpan = new TimeSpan(Convert.ToInt32(TiempoVidaCookieHoras), Convert.ToInt32(TiempoVidaCookieMinutos), Convert.ToInt32(TiempoVidaCookieSegundos)),
                DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"c:\shared-auth-ticket-keys\"))
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Login}/{id?}");
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
