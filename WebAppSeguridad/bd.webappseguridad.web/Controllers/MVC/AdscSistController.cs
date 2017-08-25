using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.entidades.Negocio;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task<IActionResult> Index()
        {
            var listado = await adscSistServicio.ListarAdscSistAsync();
         
            return View(listado);

        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return NotFound();
                }
                var respuesta = await adscSistServicio.EliminarAsync(id);
                if (!respuesta.IsSuccess)
                {
                    return BadRequest();
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}