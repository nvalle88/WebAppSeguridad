using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using bd.webappseguridad.entidades.Utils;

namespace bd.log.web.Controllers
{
   
    public class HomeController : Controller
    {
        [Authorize(Policy = PoliticasSeguridad.TienePermiso)]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
