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

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class AdscSistController : Controller
    {
        private readonly IAdscSistServicio adscSistServicio;
        private readonly IBaseDatosServicio baseDatosServicio;

        public AdscSistController(IAdscSistServicio adscSistServicio, IBaseDatosServicio baseDatosServicio)
        {
            this.adscSistServicio = adscSistServicio;
            this.baseDatosServicio = baseDatosServicio;

        }

        public async Task<IActionResult> Create()
        {

            ViewData["AdbdBdd"] = new SelectList(await baseDatosServicio.ListarBaseDatosAsync(), "AdbdBdd", "AdbdBdd");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adscsist Adscsist)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await adscSistServicio.CrearAsync(Adscsist);
                    if (response.IsSuccess)
                    {
                        return RedirectToAction("Index");
                    }

                    ViewData["Error"] = response.Message;
                }
                ViewData["AdbdBdd"] = new SelectList(await baseDatosServicio.ListarBaseDatosAsync(), "AdbdBdd", "AdbdBdd");
                return View(Adscsist);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var respuesta = await adscSistServicio.SeleccionarAsync(id);

                if (respuesta.IsSuccess)
                {
                    var Adscsist = (Adscsist)respuesta.Resultado;
                    ViewData["AdbdBdd"] = new SelectList(await baseDatosServicio.ListarBaseDatosAsync(), "AdbdBdd", "AdbdBdd", Adscsist.AdstBdd);
                    return View(respuesta.Resultado);
                }

                return NotFound();
            }
            catch (Exception)
            {
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
                    var respuesta = await adscSistServicio.EditarAsync(id, Adscsist);

                    if (respuesta.IsSuccess)
                    {
                        return RedirectToAction("Index");
                    }

                    ViewData["Error"] = respuesta.Message;
                }
                ViewData["AdbdBdd"] = new SelectList(await baseDatosServicio.ListarBaseDatosAsync(), "AdbdBdd", "AdbdBdd", Adscsist.AdstBdd);
                return View(Adscsist);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> Index(string mensaje)
        {
            var listado = await adscSistServicio.ListarAdscSistAsync();
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
                  

                    var response = await adscSistServicio.EliminarAsync(id);
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