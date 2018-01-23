using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.servicios.Servicios;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.entidades.Negocio;
using Microsoft.AspNetCore.Mvc.Rendering;
using bd.log.guardar.Servicios;
using bd.log.guardar.Inicializar;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using Newtonsoft.Json;
using bd.webappseguridad.entidades.ViewModels;
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
    public class AdscgrpsController : Controller
    {
        private readonly IApiServicio apiServicio;

        public AdscgrpsController(IApiServicio apiServicio)
        {
            
            this.apiServicio = apiServicio;
        }

        public async Task<IActionResult> Index(string mensaje)
        {
            try
            {
                var ListaAdscgrp = await apiServicio.Listar<Adscgrp>(new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarAdscgrp");
                if (mensaje==null)
                {
                    mensaje = "";
                }
                ViewData["Error"] = mensaje;
                return View(ListaAdscgrp);
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
        public async Task<IActionResult> Create(Adscgrp adscgrp)
        {
            entidades.Utils.Response response = new entidades.Utils.Response();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await apiServicio.InsertarAsync(adscgrp,
                                                                 new Uri(WebApp.BaseAddress),
                                                                 "api/Adscgrps/InsertarAdscgrp");
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
                await CargarListaBdd();
                InicializarMensaje(response.Message);
                return View(adscgrp);

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

        public async Task<IActionResult> Create(string mensaje)
        {
            try
            {
                await CargarListaBdd();
                InicializarMensaje(mensaje);
                return View();
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
                return View();
            }
        }


        public async Task<IActionResult> Edit(string adgrBdd, string adgrGrupo)
        {
            try
            {
                if (adgrBdd != null || adgrGrupo != null)
                {
                    var grupo = new Adscgrp
                    {
                        AdgrBdd = adgrBdd,
                        AdgrGrupo = adgrGrupo,
                    };
                    var respuesta = await apiServicio.SeleccionarAsync(grupo, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscgrps/SeleccionarAdscgrp");
                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscgrp>(respuesta.Resultado.ToString());
                    if (respuesta.IsSuccess)
                    {
                        return View(respuesta.Resultado);
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

        

        public async Task<IActionResult> MenusGrupo(string adgrBdd, string adgrGrupo)
        {
            try
            {
                if (adgrBdd != null || adgrGrupo != null)
                {
                    var grupo = new Adscgrp
                    {
                        AdgrBdd = adgrBdd,
                        AdgrGrupo = adgrGrupo,
                    };
                    var respuesta = await apiServicio.Listar<Adscmenu>(grupo, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscgrps/MenusGrupo");


                    var menusGrupo = new MenusGrupo
                    {
                       Adgrbdd=adgrBdd,
                       Adgrgrupo=adgrGrupo,
                       listaMenus=respuesta,
                    };

                    return View(menusGrupo);

                }
                return View(new List<Adscmiem>());
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

        private async Task CargarListaCombox()
        {

            var ListaSistema = await apiServicio.Listar<Adscmenu>(new Uri(WebApp.BaseAddress), "api/Adscmenus/ListarMenuDistinct");
            ViewData["AdexSistema"] = new SelectList(ListaSistema, "AdmeSistema", "AdmeSistema");
        }


        private async Task CargarListaCombox(Adscexe adscexe)
        {

            var ListaSistema = await apiServicio.Listar<Adscmenu>(new Uri(WebApp.BaseAddress), "api/Adscmenus/ListarMenuDistinct");
            ViewData["AdexSistema"] = new SelectList(ListaSistema, "AdmeSistema", "AdmeSistema", adscexe.AdexSistema);

            var adscMenu = new Adscmenu
            {
                AdmeSistema = adscexe.AdexSistema,
            };

            var listaAplicacion = await apiServicio.ListarAplicacionPorSistema<Adscmenu>(adscMenu, new Uri(WebApp.BaseAddress), "api/Adscmenus/ListarPadresPorSistema");
            ViewData["AdexAplicacion"] = new SelectList(listaAplicacion, "AdmeAplicacion", "AdmeAplicacion");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPermisoGrupoPost(Adscexe adscexe)
        {
            var response = new entidades.Utils.Response();
            try
            {
                if (!ModelState.IsValid)
                {

                    await CargarListaCombox(adscexe);
                    return RedirectToAction("CrearPermisoGrupo", new { adgrBdd = adscexe.AdexBdd, adgrGrupo = adscexe.AdexGrupo });
                }
                response = await apiServicio.InsertarAsync(adscexe,
                                                             new Uri(WebApp.BaseAddress),
                                                             "api/Adscexes/InsertarAdscexe");
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

                    return RedirectToAction("MenusGrupo", new { adgrBdd = adscexe.AdexBdd, adgrGrupo = adscexe.AdexGrupo});
                }
                await CargarListaCombox();
               
                return RedirectToAction("CrearPermisoGrupo", new { adgrBdd = adscexe.AdexBdd, adgrGrupo = adscexe.AdexGrupo,mensaje=response.Message });

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


        [HttpGet]
        public async Task<IActionResult> CrearPermisoGrupo(string adgrBdd, string adgrGrupo,string mensaje)
        {
            var miem = new Adscexe
            {
                AdexBdd = adgrBdd,
                AdexGrupo = adgrGrupo,
            };

            await CargarListaCombox();
            InicializarMensaje(mensaje);
            return View(miem);
        }



        [HttpGet]
        public async Task<IActionResult> CrearMiembroGrupo(string adgrBdd, string adgrGrupo,string mensaje)
        {
            var miem = new Adscmiem
            {
                AdmiBdd=adgrBdd,
                AdmiGrupo=adgrGrupo,
            };
            InicializarMensaje(mensaje);
            await CargarUsuarios();
            return   View(miem);
        }

        private async Task CargarUsuarios()
        {
            var listaUsuarios = await apiServicio.Listar<Adscpassw>(new Uri(WebApp.BaseAddress), "/api/Adscpassws/ListarAdscPassw");
            ViewData["AdpsLogin"] = new SelectList(listaUsuarios, "AdpsLogin", "AdpsLogin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearMiembroGrupoPost(Adscmiem adscmiem)
        {
            
            try
            {
                var response = new entidades.Utils.Response();
                if (ModelState.IsValid)
                {
                    response = await apiServicio.InsertarAsync(adscmiem,
                                                                         new Uri(WebApp.BaseAddress),
                                                                         "api/Adscmiems/InsertarAdscmiem");
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

                        return RedirectToAction("MiembrosGrupo", new { adgrBdd = adscmiem.AdmiBdd, adgrGrupo = adscmiem.AdmiGrupo });
                    } 
                }
                await CargarUsuarios();
                await CargarListaBdd();
                return RedirectToAction("CrearMiembroGrupo", new { adgrBdd = adscmiem.AdmiBdd, adgrGrupo = adscmiem.AdmiGrupo,mensaje=response.Message});

            }
            catch (Exception ex)
            {
                var responseLog = new EntradaLog
                {
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                    ObjectPrevious = null,
                    ObjectNext = null,
                };

                await apiServicio.SalvarLog<log.guardar.Utiles.Response>(HttpContext, responseLog);

                return BadRequest();
            }
        }

        public async Task<IActionResult> MiembrosGrupo(string adgrBdd, string adgrGrupo)
        {
            try
            {
                if (adgrBdd != null || adgrGrupo != null)
                {
                    var grupo = new Adscgrp
                    {
                        AdgrBdd = adgrBdd,
                        AdgrGrupo = adgrGrupo,
                    };
                    var respuesta = await apiServicio.Listar<Adscmiem>(grupo, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscgrps/MiembrosGrupo");

                    var miembrosGrupo = new MiembrosGrupo
                    {
                        Adgrbdd=adgrBdd,
                        Adgrgrupo=adgrGrupo,
                        ListaMiembros=respuesta,
                    };
                    return View(miembrosGrupo);

                }
                return View(new List<Adscmiem>());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Adscgrp adscgrp)
        {
            var response = new entidades.Utils.Response();
            try
            {
                if (!string.IsNullOrEmpty(adscgrp.AdgrBdd) || !string.IsNullOrEmpty(adscgrp.AdgrGrupo))
                {
                    var respuestaActualizar = await apiServicio.SeleccionarAsync(adscgrp, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscgrps/SeleccionarAdscgrp");
                    response = await apiServicio.EditarAsync(adscgrp, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscgrps/EditarAdscgrp");

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


        public async Task<IActionResult> Delete(string adgrBdd, string adgrGrupo)
        {

            try
            {
                if (adgrBdd != null || adgrGrupo != null)
                {
                    var grupo = new Adscgrp
                    {
                        AdgrBdd = adgrBdd,
                        AdgrGrupo = adgrGrupo,
                    };

                    var response = await apiServicio.EliminarAsync(grupo, new Uri(WebApp.BaseAddress)
                                                                           , "api/Adscgrps/EliminarAdscgrp");
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

       
        public async Task<JsonResult> ListarAplicacionPorSistema(string AdexSistema)
        {
            var sistema = new Adscmenu
            {
                AdmeSistema = AdexSistema,
            };
            var listaGrupos = await apiServicio.ListarAplicacionPorSistema(sistema, new Uri(WebApp.BaseAddress), "api/Adscmenus/ListarPadresPorSistema");
            return Json(listaGrupos);
        }

        private async Task CargarListaBdd()
        {
            var listaBdd = await apiServicio.Listar<Adscbdd>(new Uri(WebApp.BaseAddress), "api/BasesDatos/ListarBasesDatos");
            ViewData["AdbdBdd"] = new SelectList(listaBdd, "AdbdBdd", "AdbdBdd");
        }
    }
}