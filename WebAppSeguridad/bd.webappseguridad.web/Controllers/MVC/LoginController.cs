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

        public async Task<IActionResult> Login()
        {
            try
            {
                var user = HttpContext.User;
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
                    AdpsLoginAdm = qsList[0],
                    AdpsTokenTemp = qsList[1]
                };
                adscpassw = await GetAdscPassws(adscpasswSend);

                var a = HttpContext.Items.Count;

                if (adscpassw != null)
                {

                    var claim = HttpContext.User.Identities.Where(x => x.NameClaimType == ClaimTypes.Name).FirstOrDefault();
                    var token = claim.Claims.Where(c => c.Type == ClaimTypes.SerialNumber).FirstOrDefault().Value;
                    var NombreUsuario = claim.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

                    var claims = new[]
                        {
                    new Claim(ClaimTypes.Name,NombreUsuario),
                    new Claim(ClaimTypes.SerialNumber,token)

                };

                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));
                    // var esto= ClaimsPrincipal.Current.Identities;
                    await HttpContext.Authentication.SignInAsync("Cookies", principal, new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties { IsPersistent = true });

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
                        return RedirectToAction(nameof(HomeController.Index), "Home");
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
                return BadRequest();
            }

        }

        public async Task<IActionResult> Salir()
        {
            try
            {
                await HttpContext.Authentication.SignOutAsync("Cookies");
                return RedirectToAction(nameof(LoginController.Index), "Login");
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
                return BadRequest();
            }
        }

        public async Task<Adscpassw> GetAdscPassws (Adscpassw adscpassw)
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
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Response> EliminarTokenTemp(Adscpassw adscpassw)
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