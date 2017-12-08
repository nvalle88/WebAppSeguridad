using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.servicios.Interfaces;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using bd.webappseguridad.entidades.Utils;
using Newtonsoft.Json;
using bd.webappseguridad.entidades.ModeloTransferencia;
using System.Linq;
using System.Security.Claims;

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class BasesDatosController : Controller
    {

        private readonly IApiServicio apiServicio;

        public BasesDatosController( IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
           
        }

        public IActionResult Create(string mensaje)
        {
            InicializarMensaje(mensaje);
            return View();
        }

        private void InicializarMensaje(string mensaje)
        {
            if (mensaje == null)
            {
                mensaje = "";
            }
            ViewData["Error"] = mensaje;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adscbdd baseDato)
        {
            try
            {
                var response = new Response();
                if (ModelState.IsValid)
                {
                    response = await apiServicio.InsertarAsync(baseDato,
                                                              new Uri(WebApp.BaseAddress),
                                                              "api/BasesDatos/InsertarBaseDatos");
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
                        await apiServicio.SalvarLog<Response>(HttpContext,responseLog);
                       
                        return RedirectToAction("Index");
                    }

                }
                InicializarMensaje(response.Message);
                return View(baseDato);
            }
            catch (Exception ex )
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var baseDatos = new Adscbdd
                {
                    AdbdBdd = id,
                };
                Response respuesta = await apiServicio.SeleccionarAsync(baseDatos, new Uri(WebApp.BaseAddress),
                                                                  "api/BasesDatos/SeleccionarAdscBdd");
                respuesta.Resultado = JsonConvert.DeserializeObject<Adscbdd>(respuesta.Resultado.ToString());
                if (respuesta.IsSuccess)
                {
                     
                    return View(respuesta.Resultado);
                }

                return NotFound();
            }
            catch (Exception )
            {
                return BadRequest();

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,Adscbdd adscbdd)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var respuestaActualizar = await apiServicio.SeleccionarAsync(adscbdd, new Uri(WebApp.BaseAddress),
                                                                  "api/BasesDatos/SeleccionarAdscBdd");
                    var respuesta = await apiServicio.EditarAsync(adscbdd, new Uri(WebApp.BaseAddress),
                                                                  "api/BasesDatos/EditarAdscbdd");

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
                        await apiServicio.SalvarLog<Response>(HttpContext, responseLog);
                        return RedirectToAction("Index");
                    }

                    ViewData["Error"] = respuesta.Message;
                }
                return View(adscbdd);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> Index(string mensaje)
        {
            var Listado = await apiServicio.Listar<Adscbdd>(new Uri(WebApp.BaseAddress), "api/BasesDatos/ListarBasesDatos");
            if (mensaje == null)
            {
                mensaje = "";
            }
            ViewData["Error"] = mensaje;
            return View(Listado);
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (id != null)
                {
                    var response = await apiServicio.EliminarAsync(id,
                                                              new Uri(WebApp.BaseAddress),
                                                              "api/BasesDatos");
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
                        await apiServicio.SalvarLog<Response>(HttpContext, responseLog);

                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index", new { mensaje = response.Message });
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Eliminar Base de datos",
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });
                return BadRequest();
            }
        }
    }
}