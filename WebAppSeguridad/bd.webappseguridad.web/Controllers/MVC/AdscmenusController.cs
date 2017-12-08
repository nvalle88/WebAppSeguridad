using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.Utils;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using bd.webappseguridad.entidades.ViewModels;

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class AdscmenusController : Controller
    {
        private readonly IApiServicio apiServicio;

        public AdscmenusController(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
        }

        public async Task<IActionResult> Index(string mensaje)
        {
            try
            {
                var ListaAdscgrp = await apiServicio.Listar<Adscmenu>(new Uri(WebApp.BaseAddress), "api/Adscmenus/ListarMenu");
                if (mensaje == null)
                {
                    mensaje = "";
                }
                ViewData["Error"] = mensaje;
                return View(ListaAdscgrp);
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Listando menus",
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.NetActivity),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adscmenu adscmenu)
        {
            Response response = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                response = await apiServicio.InsertarAsync(adscmenu,
                                                             new Uri(WebApp.BaseAddress),
                                                             "api/Adscmenus/InsertarAdscmenu");
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
                        await apiServicio.SalvarLog<Response>(HttpContext, responseLog);

                        return RedirectToAction("Index");
                }
                }
                await CargarListaCombox();
                InicializarMensaje(response.Message);
                return View(adscmenu);

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Creando un menu ",
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });

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

        public async Task<IActionResult> Create(string mensaje)
        {
            await CargarListaCombox();
            InicializarMensaje(mensaje);
            return View();
        }


        public async Task<IActionResult> Details(string admeSistema, string admeAplicacion)
        {
            try
            {
                if (admeSistema != null || admeAplicacion != null)
                {
                   
                    var menu = new DetalleMenu
                    {
                        AdmeSistema = admeSistema,
                        AdmeAplicacion = admeAplicacion,
                    };
                    DetalleMenu respuesta = await apiServicio.DetalleMenuAsync(menu, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscmenus/DetalleAdscmenu");
                    if (respuesta!=null)
                    {
                        return View(respuesta);
                    }

                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> Edit(string admeSistema, string admeAplicacion)
        {
            try
            {
                if (admeSistema != null || admeAplicacion != null)
                {
                    await CargarListaPadresPorSistema(admeSistema,admeAplicacion);
                    var menu = new Adscmenu
                    {
                        AdmeSistema = admeSistema,
                        AdmeAplicacion = admeAplicacion,
                    };
                    Response respuesta = await apiServicio.SeleccionarAsync(menu, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscmenus/SeleccionarAdscMenu");
                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscmenu>(respuesta.Resultado.ToString());
                    if (respuesta.IsSuccess)
                    {
                        return View(respuesta.Resultado);
                    }

                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Adscmenu adscmenu)
        {
            Response response = new Response();
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarListaPadresPorSistema(adscmenu.AdmeSistema, adscmenu.AdmeAplicacion);
                    return View(adscmenu);
                }
                if (!string.IsNullOrEmpty(adscmenu.AdmeSistema) || !string.IsNullOrEmpty(adscmenu.AdmeAplicacion))
                {
                    response = await apiServicio.EditarAsync(adscmenu, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscmenus/EditarAdscmenu");

                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1} {2}", "Menu", adscmenu.AdmeSistema, adscmenu.AdmeAplicacion),
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            Message = "Se ha actualizado un registro Menu",
                            UserName = "Usuario 1"
                        });

                        return RedirectToAction("Index");
                    }

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Editando una Grupos",
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP "
                });

                return BadRequest();
            }
        }
        public async Task<IActionResult> DeleteHijo(string admeSistemaHijo, string admeAplicacionHijo, string admeAplicacion)
        {

            try
            {
                if (admeSistemaHijo != null || admeAplicacionHijo != null)
                {
                    var menu = new Adscmenu
                    {
                        AdmeSistema = admeSistemaHijo,
                        AdmeAplicacion = admeAplicacionHijo,
                    };

                    var response = await apiServicio.EliminarAsync(menu, new Uri(WebApp.BaseAddress)
                                                                           , "api/Adscmenus/EliminarAdscmenu");
                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1} {2}", "Menu", admeSistemaHijo, admeAplicacionHijo),
                            Message = "Registro eliminado",
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            UserName = "Usuario APP Seguridad"
                        });
                        return RedirectToAction("Details", "Adscmenus", new { admeSistema = admeSistemaHijo, admeAplicacion = admeAplicacion });
                    }
                    return BadRequest();
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

        public async Task<IActionResult> Delete(string admeSistema, string admeAplicacion)
        {

            try
            {
                if (admeSistema != null || admeAplicacion != null)
                {
                    var menu = new Adscmenu
                    {
                        AdmeSistema = admeSistema,
                        AdmeAplicacion = admeAplicacion,
                    };

                    var response = await apiServicio.EliminarAsync(menu, new Uri(WebApp.BaseAddress)
                                                                           , "api/Adscmenus/EliminarAdscmenu");
                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1} {2}", "Menu", admeSistema, admeAplicacion),
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
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });

                return BadRequest();
            }
        }

        private async Task CargarListaCombox()
        {
            var listaSistema = await apiServicio.Listar<Adscsist>(new Uri(WebApp.BaseAddress), "api/Adscsists/ListarAdscSistema");
            ViewData["AdmeSistema"] = new SelectList(listaSistema, "AdstSistema", "AdstSistema");

            //var ListaAdscgrp = await apiServicio.Listar<Adscmenu>(new Uri(WebApp.BaseAddress), "/api/Adscmenus/ListarMenu");
            //ViewData["AdmePadre"] = new SelectList(ListaAdscgrp, "AdmeAplicacion", "AdmeAplicacion");
        }

        private async Task CargarPadre(string admeSistema, string admeAplicacion)
        {
                if (admeSistema != null || admeAplicacion != null)
                {
                    await CargarListaPadresPorSistema(admeSistema, admeAplicacion);
                    var menu = new Adscmenu
                    {
                        AdmeSistema = admeSistema,
                        AdmeAplicacion = admeAplicacion,
                    };
                    Response respuesta = await apiServicio.SeleccionarAsync(menu, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscmenus/SeleccionarAdscMenu");
                    var padre = JsonConvert.DeserializeObject<Adscmenu>(respuesta.Resultado.ToString());
                    var lista = new List<Adscmenu>();
                    if (padre != null)
                    {
                        lista .Add ( new Adscmenu {AdmeAplicacion=padre.AdmeAplicacion,AdmePadre=padre.AdmePadre } );
                        ViewData["AdmePadre"] = new SelectList(lista, "AdmePadre", "AdmePadre", padre.AdmePadre);
                    }
                    

                }
        }

        private async Task CargarListaPadresPorSistema(string admesistema,string aplicacion)
        {
            var sistema = new Adscmenu
            {
                AdmeSistema = admesistema,
                AdmeAplicacion = aplicacion,
            };
            var listaPadres = await apiServicio.ListarPadresPorSistema(sistema, new Uri(WebApp.BaseAddress), "api/AdscMenus/ListarPadresPorSistema");
            Response respuesta = await apiServicio.SeleccionarAsync(sistema, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscmenus/SeleccionarAdscMenu");
            var padre = JsonConvert.DeserializeObject<Adscmenu>(respuesta.Resultado.ToString());

            if (padre.AdmePadre=="0")
            {
                ViewData["AdmePadre"] = new SelectList(listaPadres, "AdmeAplicacion", "AdmeAplicacion","Raíz");
            }
            else
            {
                ViewData["AdmePadre"] = new SelectList(listaPadres, "AdmeAplicacion", "AdmeAplicacion", padre.AdmePadre);
            }

        }

        private async Task CargarListaDePadres(string AdmeAplicacion)
        {
            var ListaAdscgrp = await apiServicio.Listar<Adscmenu>(new Uri(WebApp.BaseAddress), "api/Adscmenus/ListarMenu");
            ViewData["AdmePadre"] = new SelectList(ListaAdscgrp, "AdmeAplicacion", "AdmeAplicacion",AdmeAplicacion);
        }

        public async Task<JsonResult> ListarPadresPorSistema(string AdmeSistema)
        {
            var sistema = new Adscmenu
            {
                AdmeSistema = AdmeSistema,
            };
            var listaPadres = await apiServicio.ListarPadresPorSistema(sistema, new Uri(WebApp.BaseAddress), "api/AdscMenus/ListarPadresPorSistema");
            return Json(listaPadres);
        }
    }
}