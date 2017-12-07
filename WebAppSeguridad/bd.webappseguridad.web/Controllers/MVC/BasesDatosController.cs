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

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class BasesDatosController : Controller
    {

        private readonly IBaseDatosServicio baseDatosServicio;
        private readonly IApiServicio apiServicio;

        public BasesDatosController(IBaseDatosServicio baseDatosServicio, IApiServicio apiServicio)
        {
            this.baseDatosServicio = baseDatosServicio;
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
                    response = await baseDatosServicio.CrearAsync(baseDato);
                    if (response.IsSuccess)
                    {
                        var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(""),
                            ExceptionTrace = null,
                            Message = "Se ha creado una actividad esencial",
                            UserName = "Usuario 1",
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            EntityID = "Actividades Esenciales",
                            ObjectPrevious = "NULL",
                            ObjectNext = JsonConvert.SerializeObject(response.Resultado),
                        }
                        );
                        return RedirectToAction("Index");
                    }

                }
                InicializarMensaje(response.Message);
                return View(baseDato);
            }
            catch (Exception )
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var respuesta = await baseDatosServicio.SeleccionarAsync(id);

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

                    var resultado = await baseDatosServicio.SeleccionarAsync(id);
                    Adscbdd Response = (Adscbdd)resultado.Resultado;
                    var respuesta = await baseDatosServicio.EditarAsync(id, adscbdd);

                    if (respuesta.IsSuccess)
                    {
                        var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            ExceptionTrace = null,
                            Message =Request.Path,
                            UserName = "Irma",
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.INFO),
                            EntityID = string.Format("Datos Originales:[Base de datos:{0}|Descripción:{1}| Servidor:{2}] |||" +
                            " Datos nuevos:[Base de datos:{3}|Descripción:{4}| Servidor:{5}]"
                            , Response.AdbdBdd, Response.AdbdDescripcion, Response.AdbdServidor
                            , adscbdd.AdbdBdd, adscbdd.AdbdDescripcion, adscbdd.AdbdServidor
                            ),
                        });
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
            var listado = await baseDatosServicio.ListarBaseDatosAsync();
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


                    var response = await baseDatosServicio.EliminarAsync(id);
                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1}", "Sistema", id),
                            Message = "Registro eliminado",
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            UserName = "Irma"
                        });
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
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });
                return BadRequest();
            }
        }
    }
}