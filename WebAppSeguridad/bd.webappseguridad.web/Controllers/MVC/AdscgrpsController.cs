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

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class AdscgrpsController : Controller
    {
        private readonly IApiServicio apiServicio;

        public AdscgrpsController(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
        }

        public async Task<IActionResult> Index()
        {


            try
            {
                var ListaAdscgrp = await apiServicio.Listar<Adscgrp>(new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarAdscgrp");
                return View(ListaAdscgrp);
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Listando sistemas",
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
        public async Task<IActionResult> Create(Adscgrp adscgrp)
        {
            Response response = new Response();
            try
            {
                response = await apiServicio.InsertarAsync(adscgrp,
                                                             new Uri(WebApp.BaseAddress),
                                                             "/api/Adscgrps/InsertarAdscgrp");
                if (response.IsSuccess)
                {

                    var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                        ExceptionTrace = null,
                        Message = "Se ha creado un grupo",
                        UserName = "Usuario 1",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        EntityID = string.Format("{0} {1} {2}", "Grupo:", adscgrp.AdgrGrupo, adscgrp.AdgrBdd),
                    });

                    return RedirectToAction("Index");
                }
                await CargarListaBdd();
                ViewData["Error"] = response.Message;
                return View(adscgrp);

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Creando un grupo ",
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
            await CargarListaBdd();
            return View();
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
                    Response respuesta = await apiServicio.SeleccionarAsync(grupo, new Uri(WebApp.BaseAddress),
                                                                  "/api/Adscgrps/SeleccionarAdscgrp");
                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscgrp>(respuesta.Resultado.ToString());
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



        [HttpGet]
        public async Task<IActionResult> CrearMiembroGrupo(string adgrBdd, string adgrGrupo)
        {
            var miem = new Adscmiem
            {
                AdmiBdd=adgrBdd,
                AdmiGrupo=adgrGrupo,
            };

            return View(miem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearMiembroGrupoPost(Adscmiem adscmiem)
        {
            Response response = new Response();
            try
            {
                response = await apiServicio.InsertarAsync(adscmiem,
                                                             new Uri(WebApp.BaseAddress),
                                                             "/api/Adscmiems/InsertarAdscmiem");
                if (response.IsSuccess)
                {

                    var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                        ExceptionTrace = null,
                        Message = "Se ha creado un grupo",
                        UserName = "Usuario 1",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        EntityID = string.Format("{0} {1} {2} {3}", "Grupo:", adscmiem.AdmiEmpleado, adscmiem.AdmiGrupo, adscmiem.AdmiBdd),
                    });

                   
                    return RedirectToAction("MiembrosGrupo",new { adgrBdd = adscmiem.AdmiBdd, adgrGrupo = adscmiem.AdmiGrupo } );
                }
                await CargarListaBdd();
                ViewData["Error"] = response.Message;
                return View(adscmiem);

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Creando un grupo ",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });

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
                                                                  "/api/Adscgrps/MiembrosGrupo");

                    return View(respuesta);

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
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(adscgrp.AdgrBdd) || !string.IsNullOrEmpty(adscgrp.AdgrGrupo))
                {
                    response = await apiServicio.EditarAsync(adscgrp, new Uri(WebApp.BaseAddress),
                                                                 "/api/Adscgrps/EditarAdscgrp");

                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1} {2}", "Grupo", adscgrp.AdgrGrupo, adscgrp.AdgrBdd),
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            Message = "Se ha actualizado un registro Grupo",
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
                                                                           , "/api/Adscgrps/EliminarAdscgrp");
                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1} {2}", "Grupo", adgrGrupo, adgrBdd),
                            Message = "Registro eliminado",
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            UserName = "Usuario APP Seguridad"
                        });
                        return RedirectToAction("Index");
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

        private async Task CargarListaBdd()
        {
            var listaBdd = await apiServicio.Listar<Adscbdd>(new Uri(WebApp.BaseAddress), "api/BasesDatos/ListarBasesDatos");
            ViewData["AdbdBdd"] = new SelectList(listaBdd, "AdbdBdd", "AdbdBdd");
        }
    }
}