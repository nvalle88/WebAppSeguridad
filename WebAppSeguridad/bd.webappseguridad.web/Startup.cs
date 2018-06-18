﻿using bd.log.guardar.Inicializar;
using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.servicios.Servicios;
using bd.webappseguridad.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
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
    /// <summary>
    /// Clase donde inicia la aplicación 
    /// Para más información visitar:https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Donde inicia la aplicación, y se carga el fichero de configuración
        /// </summary>
        /// <param name="env"></param>
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
        public  void ConfigureServices(IServiceCollection services)
        {

            /// <summary>
            /// Se añaden los servicios necesarios para el funcionaminto del aplicativo.
            /// para poder utilizar la inyección de dependencia.
            /// </summary>
            services.AddMvc();
            services.AddDataProtection()
            .UseCryptographicAlgorithms(
            new AuthenticatedEncryptionSettings()
             {
             EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
             ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });

            services.AddDataProtection()
            .SetDefaultKeyLifetime
            (TimeSpan.FromDays
             (Convert.ToInt32
              (Configuration.GetSection("DiasValidosClaveEncriptada").Value)
             )
            );

            //services.AddSingleton<IAdscSistServicio, AdscSistServicio>();
            services.AddSingleton<IApiServicio, ApiServicio>();
            services.AddSingleton<IMenuServicio, MenuServicio>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationHandler, RolesHandler>();
           // services.AddSingleton<IAdscpasswServicio, AdscpasswServicio>();
            services.AddResponseCaching();

            /// <summary>
            /// Se añade a las politicas de autorización la autorización personalizada.
            /// que válida si el usuario está autenticado y si tiene acceso al recurso solicitado 
            /// esta autorización se obtiene desde la base de datos 
            /// Si el grupo del usuario está autorizado a realizar la acción que ha solicitado.
            /// </summary>
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("EstaAutorizado",
            //                      policy => policy.Requirements.Add(new RolesRequirement()));
            //});

            services.AddMvc(options =>
            {
                options.Filters.Add(new Filtro());
            });



            /// <summary>
            /// Se lee el fichero appsetting.json según las etiquetas expuestas en este.
            /// Ejemplo:HostServicioSeguridad es el host donde se encuentran los servicios de Seguridad.
            ///  ServiciosLog es el nombre con el que se encuentra en la base de datos, en la tabla adscsist
            ///  de aquí se obtiene el valor del Hostdonde se encuentra el servicio de Log .
            /// </summary>
            WebApp.BaseAddressWebAppLogin= Configuration.GetSection("HostLogin").Value;
            WebApp.NombreAplicacion = Configuration.GetSection("NombreAplicacion").Value;
            /// <summary>
            /// Se llama a la clase inicializar para darle valor a las variables donde se hospedan los servicios
            /// que utiliza la aplicación
            /// Ejemplo:WebApp.BaseAddressSeguridad es el host donde se encuentran los servicios de Seguridad.
            ///  AppGuardarLog.BaseAddress es el host donde se encuentran los servicios de Log.
            /// </summary>
            WebApp.BaseAddress= Configuration.GetSection("HostServicioSeguridad").Value;
            AppGuardarLog.BaseAddress = Configuration.GetSection("HostServicioLog").Value;

            WebApp.NivelesMenu = Convert.ToInt32(Configuration.GetSection("NivelMenu").Value);

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

            /// <summary>
            /// Es para cargar los ficheros estáticos de la aplicación como Css, imagenes etc...
            /// Configuración por defecto es en la carpeta wwwroot
            /// Para más información visitar : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files
            /// </summary>
            app.UseStaticFiles();

            /// <summary>
            /// Se lee el fichero appsetting.json según las etiquetas expuestas en este.
            /// Ejemplo:TiempoVidaCookieHoras Horas que tendra de vida la cookie.
            /// TiempoVidaCookieMinutos Minutos que tendra de vida la cookie
            ///  TiempoVidaCookieSegundos Minutos que tendra de vida la cookie.
            ///  Con estas tres variables mencionadas se conforma el tiempo de vida de la Cookie (ExpireTimeSpan)
            /// </summary>
            var TiempoVidaCookieHoras = Configuration.GetSection("TiempoVidaCookieHoras").Value;
            var TiempoVidaCookieMinutos = Configuration.GetSection("TiempoVidaCookieMinutos").Value;
            var TiempoVidaCookieSegundos = Configuration.GetSection("TiempoVidaCookieSegundos").Value;


            /// <summary>
            /// Configuración de la Cookie de autorización  
            /// </summary>
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


            /// <summary>
            /// Configuración del MVC, ruta que definimos (Controlador/Acción/Parametros)
            /// Para más información visitar:https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing
            /// </summary>
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
