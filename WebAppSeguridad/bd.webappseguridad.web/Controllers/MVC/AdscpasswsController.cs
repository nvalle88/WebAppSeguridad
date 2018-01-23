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

namespace bd.webappseguridad.web.Controllers.MVC
{
    /// <summary>
    /// Es donde se gestiona las acciones realizadas en las vistas: como mostrar vistas para crear 
    /// editar listar así como las acciones Post que llaman a los servicios web que afectan 
    /// la base de datos.
    /// Este controlador está protegido con la política de autorización "EstaAutorizado" 
    /// que es la que le da el permiso o no 
    /// de acceder al método que se solicite en el path del contexto de la aplicación 
    /// Se hace uso de la inyección de dependencia donde se inyecta la 
    /// interfaz IApiServicio y se inicializa en el contructor del controlador.
    /// Los métodos son etiquetado con anotaciones [Get] y [Post] 
    /// todos los métodos por defecto son [Get] por eso no es necesario colocarle la anotación
    /// en los métodos [Post] hay una validación de seguridad AntiForgeryToken 
    /// para más información sobre AntiForgery visitar:https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery.
    /// </summary>
    [Authorize(Policy = PoliticasSeguridad.TienePermiso)]
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


                    return RedirectToAction("Index");
                }

                ViewData["Error"] = response.Message;
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

                return BadRequest();
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var respuesta = await apiServicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                                  "/api/Adscpassws");
                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscsist>(respuesta.Resultado.ToString());
                    if (respuesta.IsSuccess)
                    {
                        return View(respuesta);
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
                return BadRequest();
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
                                                                  "/api/Adscpassws");

                    response = await apiServicio.EditarAsync(id, adscpassw, new Uri(WebApp.BaseAddress),
                                                                 "/api/Adscpassws");

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

                        return RedirectToAction("Index");
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

                return BadRequest();
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
                return BadRequest();
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

                    return RedirectToAction("Index");
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

                return BadRequest();
            }
        }
    }
}