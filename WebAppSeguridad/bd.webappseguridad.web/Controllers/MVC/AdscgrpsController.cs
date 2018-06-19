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
using bd.webappseguridad.servicios.Extensores;

namespace bd.webappseguridad.web.Controllers.MVC
{
    /// <summary>
    /// Es donde se gestiona las acciones realizadas en las vistas: como mostrar vistas para crear 
    /// editar listar as� como las acciones Post que llaman a los servicios web que afectan 
    /// la base de datos.
    /// Este controlador est� protegido con la pol�tica de autorizaci�n "EstaAutorizado" 
    /// que es la que le da el permiso o no 
    /// de acceder al m�todo que se solicite en el path del contexto de la aplicaci�n 
    /// Se hace uso de la inyecci�n de dependencia donde se inyecta la 
    /// interfaz IApiServicio y se inicializa en el contructor del controlador.
    /// Los m�todos son etiquetado con anotaciones [Get] y [Post] 
    /// todos los m�todos por defecto son [Get] por eso no es necesario colocarle la anotaci�n
    /// en los m�todos [Post] hay una validaci�n de seguridad AntiForgeryToken 
    /// para m�s informaci�n sobre AntiForgery visitar:https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery.
    /// </summary>
    //[Authorize(Policy = PoliticasSeguridad.TienePermiso)]
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
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
            }
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

                        return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }
                }
                await CargarListaBdd();
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
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
            }
        }

        public async Task<IActionResult> Create(string mensaje)
        {
            try
            {
                await CargarListaBdd();
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
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
            }
        }

        public async Task<IActionResult> EliminarMiembro(string adgrBdd, string adgrGrupo, string empleado)
        {
            try
            {
                if (adgrBdd != null || adgrGrupo != null || empleado != null )
                {
                    var miembroGrupo = new Adscmiem
                    {
                        AdmiBdd = adgrBdd,
                        AdmiGrupo = adgrGrupo,
                        AdmiEmpleado=empleado
                        
                    };
                    var respuesta = await apiServicio.EliminarAsync(miembroGrupo, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscmiems/EliminarAdscmiem");

                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscmiem>(respuesta.Resultado.ToString());

                    if (respuesta.IsSuccess)
                    {
                        var responseLog = new EntradaLog
                        {
                            ExceptionTrace = null,
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            ObjectPrevious = JsonConvert.SerializeObject(respuesta.Resultado),
                            ObjectNext = null,
                        };
                        return this.Redireccionar("Adscgrps", "MiembrosGrupo", new { adgrBdd = miembroGrupo.AdmiBdd, adgrGrupo = miembroGrupo.AdmiGrupo }, $"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }
                    return this.Redireccionar("Adscgrps", "MiembrosGrupo", new { adgrBdd = miembroGrupo.AdmiBdd, adgrGrupo = miembroGrupo.AdmiGrupo }, $"{Mensaje.Aviso}|{respuesta.Message}");

                }

                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
            }
        }

        public async Task<IActionResult> EliminarPermiso(string adgrBdd, string adgrGrupo, string sistema,string aplicacion)
        {
            try
            {
                if (adgrBdd != null || adgrGrupo != null || sistema!=null || aplicacion!=null)
                {
                    var permisoGrupo = new Adscexe
                    {
                        AdexBdd = adgrBdd,
                        AdexGrupo = adgrGrupo,
                        AdexSistema=sistema,
                        AdexAplicacion=aplicacion
                    };
                    var respuesta = await apiServicio.EliminarAsync(permisoGrupo, new Uri(WebApp.BaseAddress),
                                                                  "api/Adscexes/EliminarAdscexe");
                   
                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscexe>(respuesta.Resultado.ToString());

                    if (respuesta.IsSuccess)
                    {
                        var responseLog = new EntradaLog
                        {
                            ExceptionTrace = null,
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            ObjectPrevious = JsonConvert.SerializeObject(respuesta.Resultado),
                            ObjectNext = null,
                        };
                        return this.Redireccionar("Adscgrps", "MenusGrupo", new { adgrBdd = permisoGrupo.AdexBdd, adgrGrupo = permisoGrupo.AdexGrupo }, $"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }
                    return this.Redireccionar("Adscgrps", "MenusGrupo", new { adgrBdd = permisoGrupo.AdexBdd, adgrGrupo = permisoGrupo.AdexGrupo }, $"{Mensaje.Aviso}|{respuesta.Message}");

                }

                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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

                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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
        public async Task<IActionResult> CrearPermisoGrupoPost(AdscexeViewModel adscexeViewModel)
        {
            var response = new entidades.Utils.Response();
            var adscexe = new Adscexe();
            try
            {
                if (!ModelState.IsValid)
                {
                        adscexe = new Adscexe
                        {
                            AdexAplicacion = adscexeViewModel.AdexAplicacion,
                            Adex = adscexeViewModel.Adex,
                            AdexBdd = adscexeViewModel.AdexBdd,
                            AdexGrupo = adscexeViewModel.AdexGrupo,
                            AdexNavigation = adscexeViewModel.AdexNavigation,
                            AdexSistema = adscexeViewModel.AdexSistema,
                            AdexSql = adscexeViewModel.AdexSql,

                        };
            
                    await CargarListaCombox(adscexe);
                    return this.Redireccionar("Adscgrps", "CrearPermisoGrupo", new { adgrBdd = adscexe.AdexBdd, adgrGrupo = adscexe.AdexGrupo }, $"{Mensaje.Error}|{Mensaje.ModeloInvalido}");
                }

                int ins = 0;
                int del = 0;
                int upd = 0;
                int sel = 0;

                if (adscexeViewModel.Del == true)
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
                    Del = del,
                    Sel = sel,
                    Upd = upd,
                    Ins = ins,
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
                    return this.Redireccionar("Adscgrps", "MenusGrupo", new { adgrBdd = adscexe.AdexBdd, adgrGrupo = adscexe.AdexGrupo }, $"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                }
                await CargarListaCombox();
                return this.Redireccionar("Adscgrps", "CrearPermisoGrupo", new { adgrBdd = adscexe.AdexBdd, adgrGrupo = adscexe.AdexGrupo }, $"{Mensaje.Error}|{response.Message}");
                

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

                return this.Redireccionar("Adscgrps", "CrearPermisoGrupo", new { adgrBdd = adscexe.AdexBdd, adgrGrupo = adscexe.AdexGrupo }, $"{Mensaje.Error}|{Mensaje.Excepcion}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> CrearPermisoGrupo(string adgrBdd, string adgrGrupo,string mensaje)
        {
            var miem = new AdscexeViewModel
            {
                AdexBdd = adgrBdd,
                AdexGrupo = adgrGrupo,
            };

            await CargarListaCombox();
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
                        return this.Redireccionar("Adscgrps", "MiembrosGrupo", new { adgrBdd = adscmiem.AdmiBdd, adgrGrupo = adscmiem.AdmiGrupo }, $"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }
                }
                await CargarUsuarios();
                await CargarListaBdd();
                return this.Redireccionar("Adscgrps", "CrearMiembroGrupo", new { adgrBdd = adscmiem.AdmiBdd, adgrGrupo = adscmiem.AdmiGrupo }, $"{Mensaje.Error}|{response.Message}");

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
                return this.Redireccionar("Adscgrps", "CrearMiembroGrupo", new { adgrBdd = adscmiem.AdmiBdd, adgrGrupo = adscmiem.AdmiGrupo }, $"{Mensaje.Error}|{Mensaje.Excepcion}");
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

                        return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }

                }
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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

                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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

                        return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }
                    return this.Redireccionar($"{Mensaje.Error}|{response.Message}");
                }

                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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

               return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
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