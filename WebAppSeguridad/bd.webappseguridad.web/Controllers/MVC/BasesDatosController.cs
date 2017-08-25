using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.servicios.Interfaces;

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class BasesDatosController : Controller
    {

        private readonly IBaseDatosServicio baseDatosServicio;
       

        public BasesDatosController(IBaseDatosServicio baseDatosServicio)
        {
            this.baseDatosServicio = baseDatosServicio;
           
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adscbdd baseDato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await baseDatosServicio.CrearAsync(baseDato);
                    if (response.IsSuccess)
                    {
                        return RedirectToAction("Index");
                    }

                    ViewData["Error"] = response.Message;
                }
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

        public async Task<IActionResult> Index()
        {

            var listado = await baseDatosServicio.ListarBaseDatosAsync();
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
                var respuesta = await baseDatosServicio.EliminarAsync(id);
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