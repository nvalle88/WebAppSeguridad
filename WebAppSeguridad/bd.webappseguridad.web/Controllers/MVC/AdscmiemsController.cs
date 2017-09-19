using System;
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

namespace bd.webappseguridad.web.Controllers.MVC
{
    public class AdscmiemsController : Controller
    {
        private readonly IApiServicio apiServicio;

        public AdscmiemsController(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var ListaAdscgrp = await apiServicio.Listar<Adscmiem>(new Uri(WebApp.BaseAddress), "api/Adscmiems/ListarAdscmiem");
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
        public async Task<IActionResult> Create(Adscmiem adscmiem)
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
                        EntityID = string.Format("{0} {1} {2} {3}", "Grupo:", adscmiem.AdmiEmpleado, adscmiem.AdmiGrupo,adscmiem.AdmiBdd),
                    });

                    return RedirectToAction("Index");
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

        public async Task<IActionResult> Create()
        {
            await CargarListaBdd();
            return View();
        }


        public async Task<IActionResult> Edit(string admiBdd, string admiGrupo, string admiEmpleado)
        {
            try
            {
                if (admiBdd != null || admiGrupo != null || admiEmpleado != null)
                {
                    var grupo = new Adscmiem
                    {
                        AdmiBdd = admiBdd,
                        AdmiEmpleado= admiEmpleado,
                        AdmiGrupo = admiGrupo,
                       
                    };
                    Response respuesta = await apiServicio.SeleccionarAsync(grupo, new Uri(WebApp.BaseAddress),
                                                                  "/api/Adscmiems/SeleccionarAdscmiem");
                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscmiem>(respuesta.Resultado.ToString());
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
        public async Task<IActionResult> Edit(Adscmiem adscmiem)
        {
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(adscmiem.AdmiBdd) || !string.IsNullOrEmpty(adscmiem.AdmiGrupo) || !string.IsNullOrEmpty(adscmiem.AdmiEmpleado))
                {
                    response = await apiServicio.EditarAsync(adscmiem, new Uri(WebApp.BaseAddress),
                                                                 "/api/Adscmiems/EditarAdscmiem");

                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1} {2} {3}", "Grupo", adscmiem.AdmiEmpleado, adscmiem.AdmiGrupo,adscmiem.AdmiBdd),
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


        public async Task<IActionResult> Delete(string admiBdd, string admiGrupo, string admiEmpleado)
        {

            try
            {
                if (admiBdd != null || admiGrupo != null || admiEmpleado != null)
                {
                    var grupo = new Adscmiem
                    {
                        AdmiBdd = admiBdd,
                        AdmiEmpleado = admiEmpleado,
                        AdmiGrupo = admiGrupo,

                    };

                    var response = await apiServicio.EliminarAsync(grupo, new Uri(WebApp.BaseAddress)
                                                                           , "/api/Adscmiems/EliminarAdscmiem");
                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1} {2} {3}", "Grupo", admiEmpleado, admiGrupo,admiBdd),
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
            var listaGrupos = await apiServicio.Listar<Adscgrp>(new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarAdscgrp");
            ViewData["AdbdBdd"] = new SelectList(listaBdd, "AdbdBdd", "AdbdBdd");
            ViewData["AdbdGrp"] = new SelectList(listaGrupos, "AdgrGrupo", "AdgrGrupo");
        }


        public async Task<JsonResult> ListarBdd(string AdmiGrupo)
        {
            var grupo = new Adscgrp
            {
                AdgrGrupo=AdmiGrupo,
               AdgrBdd=null,
               
            };
           var listaGrupos = await apiServicio.Listar(grupo,new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarBddPorGrupo");
            return Json(listaGrupos);
        }

    }
}