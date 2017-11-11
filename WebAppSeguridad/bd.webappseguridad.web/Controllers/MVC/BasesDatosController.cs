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

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class BasesDatosController : Controller
    {

        private readonly IBaseDatosServicio baseDatosServicio;
       

        public BasesDatosController(IBaseDatosServicio baseDatosServicio)
        {
            this.baseDatosServicio = baseDatosServicio;
           
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
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            ExceptionTrace = null,
                            Message = "Se ha creado una base de datos",
                            UserName = "Usuario Nestor Nuevo",
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            EntityID = string.Format("{0} {1}", "Base de datos:", baseDato.AdbdBdd),
                        });
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
                    var respuesta = await baseDatosServicio.EditarAsync(id, adscbdd);

                    if (respuesta.IsSuccess)
                    {
                        return RedirectToAction("Index");
                    }

                    ViewData["Error"] = respuesta.Message;
                }
                return View(adscbdd);
            }
            catch (Exception)
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
                            UserName = "Usuario APP Seguridad"
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