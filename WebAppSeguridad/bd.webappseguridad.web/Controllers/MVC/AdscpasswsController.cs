using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.servicios.Interfaces;

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class AdscpasswsController : Controller
    {

        private readonly IAdscpasswServicio adscpasswServicio;


        public AdscpasswsController(IAdscpasswServicio adscpasswServicio)
        {
            this.adscpasswServicio = adscpasswServicio;

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adscpassw adscpassw)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await adscpasswServicio.CrearAsync(adscpassw);
                    if (response.IsSuccess)
                    {
                        return RedirectToAction("Index");
                    }

                    ViewData["Error"] = response.Message;
                }
                return View(adscpassw);
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
                var respuesta = await adscpasswServicio.SeleccionarAsync(id);

                if (respuesta.IsSuccess)
                {
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
        public async Task<IActionResult> Edit(string id, Adscpassw adscbdd)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var respuesta = await adscpasswServicio.EditarAsync(id, adscbdd);

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

        public async Task<IActionResult> Index()
        {

            var listado = await adscpasswServicio.ListarAdscPasswAsync();
            if (listado == null)
            {
                return BadRequest();
            }
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
                var respuesta = await adscpasswServicio.EliminarAsync(id);
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