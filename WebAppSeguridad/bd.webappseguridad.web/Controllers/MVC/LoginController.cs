using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using bd.log.guardar.Servicios;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.ObjectTranfer;
using bd.log.guardar.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.entidades.Negocio;
using bd.log.web.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace bd.webappseguridad.web.Controllers.MVC
{

    public class LoginController : Controller
    {

        private readonly IApiServicio apiServicio;


        public LoginController(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;

        }

        private void InicializarMensaje(string mensaje)
        {
            if (mensaje == null)
            {
                mensaje = "";
            }
            ViewData["Error"] = mensaje;
        }

        public IActionResult Index(string mensaje, string returnUrl = null)
        {
            InicializarMensaje(mensaje);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Método que es invocado desde la aplicaciín de Login
        /// Donde se valida el token temporal que el generado por la aplicación de Login para el usuario actual
        /// Si el token temporal es válido se elimina sino lo enviá a la aplicación de Login
        /// Si todo es satisfactorio se autentica a la cookie...
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Login()

        {
            try
            {

                //var claim = HttpContext.User.Identities.Where(x => x.NameClaimType == ClaimTypes.Name).FirstOrDefault();
                //var token = claim.Claims.Where(c => c.Type == ClaimTypes.SerialNumber).FirstOrDefault().Value;
                //var NombreUsuario = claim.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

                //var permiso = new PermisoUsuario
                //{
                //    Contexto = HttpContext.Request.Path,
                //    Token = token,
                //    Usuario = NombreUsuario,
                //};

                ///// <summary>
                ///// Se valida que la información del usuario actual tenga permiso para acceder al path solicitado... 
                ///// </summary>
                ///// <returns></returns>
                //var respuesta = apiServicio.ObtenerElementoAsync1<Response>(permiso, new Uri(WebApp.BaseAddress), "api/Adscpassws/TienePermiso");

                //if (!respuesta.Result.IsSuccess)
                //{
                //    return Redirect(WebApp.BaseAddressWebAppLogin);
                //}

                if (Request.Query.Count != 2)
                {
                    return Redirect(WebApp.BaseAddressWebAppLogin);
                }

                Adscpassw adscpassw = new Adscpassw();
                var queryStrings = Request.Query;
                var qsList = new List<string>();
                foreach (var key in queryStrings.Keys)
                {
                    qsList.Add(queryStrings[key]);
                }
                var adscpasswSend = new Adscpassw
                {
                    AdpsLogin = qsList[0],
                    AdpsTokenTemp = qsList[1]
                };
                adscpassw = await GetAdscPassws(adscpasswSend);

                if (adscpassw != null)
                {
                    var response = await EliminarTokenTemp(adscpassw);
                    if (response.IsSuccess)
                    {
                        var responseLog = new EntradaLog
                        {
                            ExceptionTrace = null,
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Permission),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.INFO),
                            ObjectPrevious = null,
                            ObjectNext = JsonConvert.SerializeObject(response.Resultado),
                        };
                        await apiServicio.SalvarLog<entidades.Utils.Response>(HttpContext, responseLog);
                        return RedirectToActionPermanent(nameof(HomeController.Index), "Home");
                    }
                    else
                    {
                        return Redirect(WebApp.BaseAddressWebAppLogin);
                    }
                }

                return Redirect(WebApp.BaseAddressWebAppLogin);
            }
            catch (Exception ex)
            {
                var responseLog = new EntradaLog
                {
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    ObjectPrevious = null,
                    ObjectNext = null,
                };
                await apiServicio.SalvarLog<entidades.Utils.Response>(HttpContext, responseLog);
                return Redirect(WebApp.BaseAddressWebAppLogin);
            }

        }

        /// <summary>
        /// Elimina el Token de la base de datos y desautentica al usuario de la Cookie
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Salir()
        {

            try
            {
                var claim = HttpContext.User.Identities.Where(x => x.NameClaimType == ClaimTypes.Name).FirstOrDefault();
                var token = claim.Claims.Where(c => c.Type == ClaimTypes.SerialNumber).FirstOrDefault().Value;
                var NombreUsuario = claim.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

                var adscpasswSend = new Adscpassw
                {
                    AdpsLogin = NombreUsuario,
                    AdpsToken = token
                };

                Adscpassw adscpassw = new Adscpassw();
                adscpassw = await GetAdscPassws(adscpasswSend);
                var response = await EliminarToken(adscpassw);
                if (response.IsSuccess)
                {
                    await HttpContext.Authentication.SignOutAsync("Cookies");
                    var responseLog = new EntradaLog
                    {
                        ExceptionTrace = null,
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Permission),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.INFO),
                        ObjectPrevious = null,
                        ObjectNext = null,
                    };
                    await apiServicio.SalvarLog<entidades.Utils.Response>(HttpContext, responseLog);
                    foreach (var cookie in HttpContext.Request.Cookies.Keys)
                    {
                        HttpContext.Response.Cookies.Delete(cookie);
                    }
                    return RedirectPermanent(WebApp.BaseAddressWebAppLogin);
                }
                return RedirectPermanent(WebApp.BaseAddressWebAppLogin);
            }
            catch (Exception)
            {
                foreach (var cookie in HttpContext.Request.Cookies.Keys)
                {
                    HttpContext.Response.Cookies.Delete(cookie);
                }
                return RedirectToAction(nameof(LoginController.Index), "Login");
            }
           
        }

        private async Task<Adscpassw> GetAdscPassws(Adscpassw adscpassw)
        {
            try
            {
                if (!adscpassw.Equals(null))
                {
                    var respuesta = await apiServicio.ObtenerElementoAsync1<Response>(adscpassw, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscpassws/SeleccionarMiembroLogueado");

                    if (respuesta.IsSuccess)
                    {
                        var obje = JsonConvert.DeserializeObject<Adscpassw>(respuesta.Resultado.ToString());
                        return obje;
                    }

                }

                return null;
            }
            catch (Exception )
            {
                return null;
            }
        }

        private async Task<Response> EliminarToken(Adscpassw adscpassw)
        {
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(adscpassw.AdpsLogin))
                {
                    response = await apiServicio.EditarAsync<Response>(adscpassw, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscpassws/EliminarToken");

                    if (response.IsSuccess)
                    {
                        return response;
                    }

                }
                return null;
            }
            catch (Exception )
            {
                return null;
            }
        }

        private async Task<Response> EliminarTokenTemp(Adscpassw adscpassw)
        {
            Response response = new Response();

            if (!string.IsNullOrEmpty(adscpassw.AdpsLogin))
            {
                response = await apiServicio.EditarAsync<Response>(adscpassw, new Uri(WebApp.BaseAddress),
                                                             "api/Adscpassws/EliminarTokenTemp");

                if (response.IsSuccess)
                {
                    return response;
                }
            }
            return null;

        }


    }
}