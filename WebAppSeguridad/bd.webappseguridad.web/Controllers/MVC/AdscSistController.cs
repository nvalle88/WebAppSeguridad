using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.entidades.Negocio;
using Microsoft.AspNetCore.Mvc.Rendering;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using bd.webappseguridad.entidades.Utils;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
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
    //[Authorize(Policy = PoliticasSeguridad.TienePermiso)]
    public class AdscSistController : Controller
    {
        
        private readonly IApiServicio apiServicio;

        public AdscSistController( IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
           

        }

        public async Task<IActionResult> Create(string mensaje)
        {
          
            var Listado = await apiServicio.Listar<Adscbdd>(new Uri(WebApp.BaseAddress), "api/BasesDatos/ListarBasesDatos");
            ViewData["AdbdBdd"] = new SelectList(Listado, "AdbdBdd", "AdbdBdd");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adscsist Adscsist)
        {
            try
            {
                var response = new Response();
                if (ModelState.IsValid)
                {
                    response = await apiServicio.InsertarAsync(Adscsist,
                                                             new Uri(WebApp.BaseAddress),
                                                             "api/Adscsists/InsertarAdscSist");
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

                    
                }
               
                var Listado = await apiServicio.Listar<Adscbdd>(new Uri(WebApp.BaseAddress), "api/BasesDatos/ListarBasesDatos");
                ViewData["AdbdBdd"] = new SelectList(Listado, "AdbdBdd", "AdbdBdd");
                TempData["Mensaje"] = $"{Mensaje.Aviso}|{response.Message}";
                return View(Adscsist);
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
                var respuesta = await apiServicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscsists");


                respuesta.Resultado = JsonConvert.DeserializeObject<Adscsist>(respuesta.Resultado.ToString());

                if (respuesta.IsSuccess)
                {
                    var Adscsist = (Adscsist)respuesta.Resultado;
                    var Listado = await apiServicio.Listar<Adscbdd>(new Uri(WebApp.BaseAddress), "api/BasesDatos/ListarBasesDatos");
                    ViewData["AdbdBdd"] = new SelectList(Listado, "AdbdBdd", "AdbdBdd");
                    return View(respuesta.Resultado);
                }

                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Error}");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Adscsist Adscsist)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var respuestaActualizar = await apiServicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscsists");

                    var respuesta = await apiServicio.EditarAsync(id, Adscsist, new Uri(WebApp.BaseAddress),
                                                                "api/Adscsists");
                     
                    if (respuesta.IsSuccess)
                    {
                        var responseLog = new EntradaLog
                        {
                            ExceptionTrace = null,
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            ObjectPrevious = JsonConvert.SerializeObject(respuestaActualizar.Resultado),
                            ObjectNext = JsonConvert.SerializeObject(respuesta.Resultado),
                        };
                        await apiServicio.SalvarLog<entidades.Utils.Response>(HttpContext, responseLog);

                        return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }

                    TempData["Mensaje"] = $"{Mensaje.Error}|{respuesta.Message}";
                }
                var Listado = await apiServicio.Listar<Adscbdd>(new Uri(WebApp.BaseAddress), "api/BasesDatos/ListarBasesDatos");
                ViewData["AdbdBdd"] = new SelectList(Listado, "AdbdBdd", "AdbdBdd");
                return View(Adscsist);
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

        public async Task<IActionResult> Index(string mensaje)
        {
           var listado = await apiServicio.Listar<Adscsist>(new Uri(WebApp.BaseAddress)
                                                                    , "api/Adscsists/ListarAdscSistema");
            return View(listado);
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (id != null)
                {

                   var response = await apiServicio.EliminarAsync(id,
                                                           new Uri(WebApp.BaseAddress),
                                                           "api/Adscsists");
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
                    return this.Redireccionar($"{Mensaje.Aviso}|{response.Message}");
                }

                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Error}");
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