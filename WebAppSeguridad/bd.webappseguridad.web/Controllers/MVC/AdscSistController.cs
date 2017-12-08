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

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class AdscSistController : Controller
    {
        
        private readonly IApiServicio apiServicio;

        public AdscSistController( IApiServicio apiServicio)
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
        public async Task<IActionResult> Create(string mensaje)
        {
            InicializarMensaje(mensaje);
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

                        return RedirectToAction("Index");
                    }

                    
                }
                InicializarMensaje(response.Message);
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
                return BadRequest();
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

                return NotFound();
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

                        return RedirectToAction("Index");
                    }

                    ViewData["Error"] = respuesta.Message;
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
                return BadRequest();
            }
        }

        public async Task<IActionResult> Index(string mensaje)
        {
           var listado = await apiServicio.Listar<Adscsist>(new Uri(WebApp.BaseAddress)
                                                                    , "api/Adscsists/ListarAdscSistema");
            if (mensaje == null)
            {
                mensaje = "";
            }
            ViewData["Error"] = mensaje;
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

                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index", new { mensaje = response.Message });
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