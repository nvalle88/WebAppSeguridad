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
using Microsoft.AspNetCore.Authorization;
using bd.webappseguridad.entidades.ViewModels;

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
    //[Authorize(Policy = PoliticasSeguridad.TienePermiso)]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdscexeViewModel adscexeViewModel)
        {
            entidades.Utils.Response response = new entidades.Utils.Response();
            try
            {
                var adscexe = new Adscexe();
                if (!ModelState.IsValid)
                {
                    adscexe = new Adscexe
                    {
                        AdexAplicacion=adscexeViewModel.AdexAplicacion,
                        Adex=adscexeViewModel.Adex,
                        AdexBdd=adscexeViewModel.AdexBdd,
                        AdexGrupo=adscexeViewModel.AdexGrupo,
                        AdexNavigation=adscexeViewModel.AdexNavigation,
                        AdexSistema=adscexeViewModel.AdexSistema,
                        AdexSql=adscexeViewModel.AdexSql,
                        
                    };
                    await CargarListaCombox(adscexe);
                    return View(adscexe);
                }


                int ins = 0;
                int del = 0;
                int upd = 0;
                int sel = 0;

                if (adscexeViewModel.Del==true)
                {
                    del = 1;
                }
                if (adscexeViewModel.Ins == true)
                {
                    ins = 1;
                }
                if (adscexeViewModel.Upd == true)
                {
                    upd = 1;
                }
                if (adscexeViewModel.Sel == true)
                {
                    sel = 1;
                }

                adscexe = new Adscexe
                {
                    AdexAplicacion = adscexeViewModel.AdexAplicacion,
                    Adex = adscexeViewModel.Adex,
                    AdexBdd = adscexeViewModel.AdexBdd,
                    AdexGrupo = adscexeViewModel.AdexGrupo,
                    AdexNavigation = adscexeViewModel.AdexNavigation,
                    AdexSistema = adscexeViewModel.AdexSistema,
                    AdexSql = adscexeViewModel.AdexSql,
                    Del=del,
                    Sel=sel,
                    Upd=upd,
                    Ins=ins,
                };

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
                    return RedirectToAction("Index");
                }
                await CargarListaCombox();
                ViewData["Error"] = response.Message;
                return View(adscexe);

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

        public async Task<IActionResult> Create()
        {
            try
            {
                var adscexe = new AdscexeViewModel
                {
                    Del = false,
                    Sel = false,
                    Upd = false,
                    Ins = false,
                };
                await CargarListaCombox();
                return View(adscexe);
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
                    entidades.Utils.Response respuesta = await apiServicio.SeleccionarAsync(menu, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscexes/SeleccionarAdscexe");
                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscmenu>(respuesta.Resultado.ToString());
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Adscmenu adscmenu)
        {
            entidades.Utils.Response response = new entidades.Utils.Response();
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(adscmenu);
                }
                if (!string.IsNullOrEmpty(adscmenu.AdmeSistema) || !string.IsNullOrEmpty(adscmenu.AdmeAplicacion))
                {

                    var respuestaActualizar = await apiServicio.SeleccionarAsync(adscmenu, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscexes/SeleccionarAdscBdd");
                    response = await apiServicio.EditarAsync(adscmenu, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscexes/EditarAdscexe");

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
                        var responseLog = new EntradaLog
                        {
                            ExceptionTrace = null,
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            ObjectPrevious = JsonConvert.SerializeObject(response.Resultado),
                            ObjectNext = null,
                        };
                        await apiServicio.SalvarLog<entidades.Utils.Response>(HttpContext, responseLog);

                        return RedirectToAction("Details", "Adscmenus", new { admeSistema = admeSistemaHijo, admeAplicacion = admeAplicacion });
                    }
                    return BadRequest();
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

        private async Task CargarListaCombox()
        {

            var ListaBDD = await apiServicio.Listar<Adscgrp>(new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarAdscgrpDistinct");
            ViewData["AdexBdd"] = new SelectList(ListaBDD, "AdgrBdd", "AdgrBdd");

            var ListaSistema = await apiServicio.Listar<Adscmenu>(new Uri(WebApp.BaseAddress), "api/Adscmenus/ListarMenuDistinct");
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
            try
            {
                var sistema = new Adscgrp
                {
                    AdgrBdd = AdexBdd,
                };
                var listaGrupos = await apiServicio.ListarGrupoPorBdd(sistema, new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarGrupoPorBdd");
                return Json(listaGrupos);
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
                return Json("Error");
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
    }
}