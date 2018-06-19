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
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using bd.webappseguridad.servicios.Extensores;

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
    public class AdscmiemsController : Controller
    {
        private readonly IApiServicio apiServicio;

        public AdscmiemsController(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
        }

        public async Task<IActionResult> Index(string mensaje)
        {
            try
            {
                var ListaAdscgrp = await apiServicio.Listar<Adscmiem>(new Uri(WebApp.BaseAddress), "api/Adscmiems/ListarAdscmiem");
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
        public async Task<IActionResult> Create(Adscmiem adscmiem)
        {
            try
            {
                var response = new Response();

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

                        return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    } 
                }

                await CargarUsuarios();
                await CargarListaBdd();
                await CargarListaBddPorGrupo(adscmiem.AdmiGrupo);
               
                return View(adscmiem);

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
               
                return View(adscmiem);
            }
        }
        
        public async Task<IActionResult> Create(string mensaje)
        {
            try
            {
                await CargarListaBdd();
                await CargarUsuarios();
                
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

        private async Task CargarUsuarios()
        {
            var listaUsuarios = await apiServicio.Listar<Adscpassw>(new Uri(WebApp.BaseAddress), "/api/Adscpassws/ListarAdscPassw");
            ViewData["AdpsLogin"] = new SelectList(listaUsuarios, "AdpsLogin", "AdpsLogin");
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
                                                                  "api/Adscmiems/SeleccionarAdscmiem");
                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscmiem>(respuesta.Resultado.ToString());
                    if (respuesta.IsSuccess)
                    {
                        return View(respuesta.Resultado);
                    }

                }

                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Satisfactorio}");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Adscmiem adscmiem)
        {
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(adscmiem.AdmiBdd) || !string.IsNullOrEmpty(adscmiem.AdmiGrupo) || !string.IsNullOrEmpty(adscmiem.AdmiEmpleado))
                {
                    var respuestaActualizar = await apiServicio.SeleccionarAsync(adscmiem, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscmiems/SeleccionarAdscmiem");

                    response = await apiServicio.EditarAsync(adscmiem, new Uri(WebApp.BaseAddress),
                                                                 "api/Adscmiems/EditarAdscmiem");

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
                                                                           , "api/Adscmiems/EliminarAdscmiem");
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


        private async Task CargarListaBddPorGrupo(string admiGrupo)
        {
            var grupo = new Adscgrp
            {
                AdgrGrupo = admiGrupo,
                AdgrBdd = null,

            };
            var listaBdd = await apiServicio.Listar<Adscbdd>(grupo, new Uri(WebApp.BaseAddress), "api/Adscgrps/ListarBddPorGrupo");
            ViewData["AdbdBdd"] = new SelectList(listaBdd, "AdbdBdd", "AdbdBdd");
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