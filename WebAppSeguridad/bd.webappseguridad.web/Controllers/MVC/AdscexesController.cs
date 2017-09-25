using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.servicios.Interfaces;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.Utils;
using bd.log.guardar.ObjectTranfer;
using bd.log.guardar.Servicios;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class AdscexesController : Controller
    {
        private readonly IApiServicio apiServicio;

        public AdscexesController(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
        }

        public async Task<IActionResult> Index(string mensaje)
        {
            try
            {
                var ListaAdscexe = await apiServicio.Listar<Adscexe>(new Uri(WebApp.BaseAddress), "api/Adscexes/ListarAdscexe");
                return View(ListaAdscexe);
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Listando Permisos",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.NetActivity),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adscexe adscexe)
        {
            Response response = new Response();
            try
            {
                if (!ModelState.IsValid)
                {

                    await CargarListaCombox(adscexe);
                    return View(adscexe);
                }
                response = await apiServicio.InsertarAsync(adscexe,
                                                             new Uri(WebApp.BaseAddress),
                                                             "api/Adscexes/InsertarAdscexe");
                if (response.IsSuccess)
                {

                    var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                        ExceptionTrace = null,
                        Message = "Se ha creado un menú",
                        UserName = "Usuario 1",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        EntityID = string.Format("{0} {1} {2} {3} {4}", "AdscExe:", adscexe.AdexSistema, adscexe.AdexAplicacion,adscexe.AdexBdd,adscexe.AdexGrupo),
                    });

                    return RedirectToAction("Index");
                }
                await CargarListaCombox();
                ViewData["Error"] = response.Message;
                return View(adscexe);

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Creando un menu ",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });

                return BadRequest();
            }
        }

        public async Task<IActionResult> Create()
        {
            var adscexe = new Adscexe
            {
                Del = false,
                Sel=false,
                Upd=false,
                Ins=false,
            };
            await CargarListaCombox();
            return View(adscexe);
        }


        public async Task<IActionResult> Details(string admeSistema, string admeAplicacion)
        {
            try
            {
                if (admeSistema != null || admeAplicacion != null)
                {

                 

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
                    var menu = new Adscmenu
                    {
                        AdmeSistema = admeSistema,
                        AdmeAplicacion = admeAplicacion,
                    };
                    Response respuesta = await apiServicio.SeleccionarAsync(menu, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscexes/SeleccionarAdscexe");
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
                    return View(adscmenu);
                }
                if (!string.IsNullOrEmpty(adscmenu.AdmeSistema) || !string.IsNullOrEmpty(adscmenu.AdmeAplicacion))
                {
                    response = await apiServicio.EditarAsync(adscmenu, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscexes/EditarAdscexe");

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
                    ExceptionTrace = ex,
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
                                                                           , "api/Adscexes/EliminarAdscexe");
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
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });

                return BadRequest();
            }
        }

        public async Task<IActionResult> Delete(string sistema, string baseDatos,string grupo, string aplicacion)
        {

            try
            {
                if (sistema != null || baseDatos != null || grupo != null || grupo != null)
                {
                    var permiso = new Adscexe
                    {
                        AdexSistema = sistema,
                        AdexBdd = baseDatos,
                        AdexGrupo=grupo,
                        AdexAplicacion=aplicacion
                    };

                    var response = await apiServicio.EliminarAsync(permiso, new Uri(WebApp.BaseAddress)
                                                                           , "api/Adscexes/EliminarAdscexe");
                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1} {2} {3} {4}", "Permiso", sistema, baseDatos,grupo,aplicacion),
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

        private async Task CargarListaCombox()
        {

            var ListaBDD = await apiServicio.Listar<Adscgrp>(new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarAdscgrpDistinct");
            ViewData["AdexBdd"] = new SelectList(ListaBDD, "AdgrBdd", "AdgrBdd");

            var ListaSistema = await apiServicio.Listar<Adscmenu>(new Uri(WebApp.BaseAddress), "/api/Adscmenus/ListarMenuDistinct");
            ViewData["AdexSistema"] = new SelectList(ListaSistema, "AdmeSistema", "AdmeSistema");
        }

        private async Task CargarListaCombox(Adscexe adscexe)
        {

            var ListaBDD = await apiServicio.Listar<Adscgrp>(new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarAdscgrpDistinct");
            ViewData["AdexBdd"] = new SelectList(ListaBDD, "AdgrBdd", "AdgrBdd",adscexe.AdexBdd);

            var adscGrupo = new Adscgrp
            {
                AdgrBdd = adscexe.AdexBdd,
            };

            var listaGrupo = await apiServicio.ListarGrupoPorBdd<Adscgrp>(adscGrupo, new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarGrupoPorBdd");
            ViewData["AdexGrupo"] = new SelectList(listaGrupo, "AdgrGrupo", "AdgrGrupo");



            var ListaSistema = await apiServicio.Listar<Adscmenu>(new Uri(WebApp.BaseAddress), "/api/Adscmenus/ListarMenuDistinct");
            ViewData["AdexSistema"] = new SelectList(ListaSistema, "AdmeSistema", "AdmeSistema",adscexe.AdexSistema);

            var adscMenu = new Adscmenu
            {
                AdmeSistema = adscexe.AdexSistema,
            };

            var listaAplicacion = await apiServicio.ListarAplicacionPorSistema<Adscmenu>(adscMenu, new Uri(WebApp.BaseAddress), "api/Adscmenus/ListarPadresPorSistema");
            ViewData["AdexAplicacion"] = new SelectList(listaAplicacion, "AdmeAplicacion", "AdmeAplicacion");

           
        }

        public async Task<JsonResult> ListarBasesDatos(string AdexBdd)
        {
            var sistema = new Adscgrp
            {
                AdgrBdd = AdexBdd,
            };
            var listaGrupos = await apiServicio.ListarGrupoPorBdd(sistema, new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarGrupoPorBdd");
            return Json(listaGrupos);
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
    }
}