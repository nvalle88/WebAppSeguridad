using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.servicios.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace bd.webappseguridad.web.Models
{

    public class Filtro : IActionFilter
    {
        private readonly IApiServicio apiServicio;

        public Filtro(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
              
                /// <summary>
                /// Se obtiene el contexto de datos 
                /// </summary>
                /// <returns></returns>
                /// con
                var httpContext = context.HttpContext;
                
                /// <summary>
                /// Se obtiene el path solicitado 
                /// </summary>
                /// <returns></returns>
                var request = httpContext.Request;


                /// <summary>
                /// Se obtiene información del usuario autenticado
                /// </summary>
                /// <returns></returns>
                var claim = context.HttpContext.User.Identities.Where(x => x.NameClaimType == ClaimTypes.Name).FirstOrDefault();
                var token = claim.Claims.Where(c => c.Type == ClaimTypes.SerialNumber).FirstOrDefault().Value;
                var NombreUsuario = claim.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

                var permiso = new PermisoUsuario
                {
                    Contexto = request.Path,
                    Token = token,
                    Usuario = NombreUsuario,
                };

                /// <summary>
                /// Se valida que la información del usuario actual tenga permiso para acceder al path solicitado... 
                /// </summary>
                /// <returns></returns>
                
                var respuestaToken = apiServicio.ObtenerElementoAsync1<Response>(permiso, new Uri(WebApp.BaseAddress), "api/Adscpassws/ExisteToken");

                if (!respuestaToken.Result.IsSuccess)
                {
                    context.HttpContext.Authentication.SignOutAsync("Cookies");
                    foreach (var cookie in context.HttpContext.Request.Cookies.Keys)
                    {
                        context.HttpContext.Response.Cookies.Delete(cookie);
                    }
                    var result = new ViewResult { ViewName = "SeccionCerrada" };
                    context.Result = result;
                }
                else
                {
                    var respuesta = apiServicio.ObtenerElementoAsync1<Response>(permiso, new Uri(WebApp.BaseAddress), "api/Adscpassws/TienePermiso");

                    //respuesta.Result.IsSuccess = true;
                    if (!respuesta.Result.IsSuccess)
                    {
                        var result = new ViewResult { ViewName = "AccesoDenegado" };
                        context.Result = result;
                    }
                }

               

            }
            catch (Exception)
            {
                var result = new RedirectResult(WebApp.BaseAddressWebAppLogin);
                foreach (var cookie in context.HttpContext.Request.Cookies.Keys)
                {
                    context.HttpContext.Response.Cookies.Delete(cookie);
                }
                context.Result = result;

            }
           
        }
    }
}
