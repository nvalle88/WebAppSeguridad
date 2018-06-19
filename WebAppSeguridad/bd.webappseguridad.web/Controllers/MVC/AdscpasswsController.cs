using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.entidades.Utils;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using bd.webappseguridad.servicios.Extensores;

namespace bd.webappseguridad.web.Controllers.MVC
{
    /// <summary>
    /// Es donde se gestiona las acciones realizadas en las vistas: como mostrar vistas para crear 
    /// editar listar as� como las acciones Post que llaman a los servicios web que afectan 
    /// la base de datos.
    /// Este controlador est� protegido con la pol�tica de autorizaci�n "EstaAutorizado" 
    /// que es la que le da el permiso o no 
    /// de acceder al m�todo que se solicite en el path del contexto de la aplicaci�n 
    /// Se hace uso de la inyecci�n de dependencia donde se inyecta la 
    /// interfaz IApiServicio y se inicializa en el contructor del controlador.
    /// Los m�todos son etiquetado con anotaciones [Get] y [Post] 
    /// todos los m�todos por defecto son [Get] por eso no es necesario colocarle la anotaci�n
    /// en los m�todos [Post] hay una validaci�n de seguridad AntiForgeryToken 
    /// para m�s informaci�n sobre AntiForgery visitar:https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery.
    /// </summary>
   // [Authorize(Policy = PoliticasSeguridad.TienePermiso)]
    public class AdscpasswsController : Controller
    {

        private readonly IApiServicio apiServicio;


        public AdscpasswsController(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adscpassw adscpassw)
        {
            Response response = new Response();
            try
            {
                var claim = HttpContext.User.Identities.Where(x => x.NameClaimType == ClaimTypes.Name).FirstOrDefault();
                var NombreUsuario = claim.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

                adscpassw.AdpsLoginAdm = NombreUsuario.ToUpper();
                response = await apiServicio.InsertarAsync(adscpassw,
                                                             new Uri(WebApp.BaseAddress),
                                                             "/api/Adscpassws/InsertarAdscPassw");
                if (response.IsSuccess)
                {
                    var responseLog = new EntradaLog
                    {
                        ExceptionTrace = null,
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        ObjectPrevious = null,
                        ObjectNext = JsonConvert.SerializeObject(response.Resultado),
                    };
                    await apiServicio.SalvarLog<entidades.Utils.Response>(HttpContext, responseLog);
                    return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                }

                TempData["Mensaje"] = $"{Mensaje.Aviso}|{response.Message}";
                return View(adscpassw);

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

                return this.Redireccionar($"{Mensaje.Error}|{ex.Message}");
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var respuesta = await apiServicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscpassws");
                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscpassw>(respuesta.Resultado.ToString());
                    if (respuesta.IsSuccess)
                    {
                        return View(respuesta.Resultado);
                    }

                }

                return BadRequest();
            }
            catch (Exception ex )
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
                return this.Redireccionar($"{Mensaje.Error}|{ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Adscpassw adscpassw)
        {
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var respuestaActualizar = await apiServicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscpassws");

                    var claim = HttpContext.User.Identities.Where(x => x.NameClaimType == ClaimTypes.Name).FirstOrDefault();
                    var NombreUsuario = claim.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

                    adscpassw.AdpsLoginAdm = NombreUsuario.ToUpper();

                    response = await apiServicio.EditarAsync(id, adscpassw, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscpassws");

                    if (response.IsSuccess)
                    {
                        var responseLog = new EntradaLog
                        {
                            ExceptionTrace = null,
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            ObjectPrevious = JsonConvert.SerializeObject(respuestaActualizar.Resultado),
                            ObjectNext = JsonConvert.SerializeObject(response.Resultado),
                        };
                        await apiServicio.SalvarLog<entidades.Utils.Response>(HttpContext, responseLog);

                        return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }

                }
                return BadRequest();
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

                return this.Redireccionar($"{Mensaje.Error}|{ex.Message}");
            }
        }

        public async Task<IActionResult> Index()
        {

            var lista = new List<Adscpassw>();
            try
            {
                lista = await apiServicio.Listar<Adscpassw>(new Uri(WebApp.BaseAddress)
                                                                    , "/api/Adscpassws/ListarAdscPassw");
                return View(lista);
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
                return this.Redireccionar($"{Mensaje.Error}|{ex.Message}");
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
           
            try
            {
               var response = await apiServicio.EliminarAsync(id,new Uri(WebApp.BaseAddress)
                                                              ,"/api/Adscpassws");
                if (response.IsSuccess)
                {
                    var responseLog = new EntradaLog
                    {
                        ExceptionTrace = null,
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        ObjectPrevious = JsonConvert.SerializeObject(response.Resultado),
                        ObjectNext = null,
                    };
                    await apiServicio.SalvarLog<entidades.Utils.Response>(HttpContext, responseLog);

                    return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                }
                return this.Redireccionar($"{Mensaje.Error}|{response.Message}");
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

                return this.Redireccionar($"{Mensaje.Error}|{ex.Message}");
            }
        }
    }
}