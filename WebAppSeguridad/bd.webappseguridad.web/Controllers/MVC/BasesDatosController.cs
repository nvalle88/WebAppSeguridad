using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.servicios.Interfaces;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using bd.webappseguridad.entidades.Utils;
using Newtonsoft.Json;
using bd.webappseguridad.entidades.ModeloTransferencia;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using bd.webappseguridad.servicios.Extensores;

namespace bd.webappseguridad.web.Controllers.MVC
{
    /// <summary>
    /// Es donde se gestiona todo sobre las base de datos como mostrar vistas para crear 
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
   // [Authorize(Policy = PoliticasSeguridad.TienePermiso)]
    public class BasesDatosController : Controller
    {
      
        private readonly IApiServicio apiServicio;

        public BasesDatosController( IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
           
        }
        
        public IActionResult Create(string mensaje)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adscbdd baseDato)
        {
            try
            {
                var response = new entidades.Utils.Response();
                if (ModelState.IsValid)
                {
                    response = await apiServicio.InsertarAsync(baseDato,
                                                              new Uri(WebApp.BaseAddress),
                                                              "api/BasesDatos/InsertarBaseDatos");
                    if (response.IsSuccess)
                    {
                        try
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
                        }
                        catch (Exception)
                        {
                            return this.Redireccionar($"{Mensaje.Informacion}|{response.Message}");
                        }

                        return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }

                }
                TempData["Mensaje"] = $"{Mensaje.Aviso}|{response.Message}";
                return View(baseDato);
            }
            catch (Exception ex )
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

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var baseDatos = new Adscbdd
                {
                    AdbdBdd = id,
                };
                var respuesta = await apiServicio.SeleccionarAsync(baseDatos, new Uri(WebApp.BaseAddress),
                                                                  "api/BasesDatos/SeleccionarAdscBdd");
                respuesta.Resultado = JsonConvert.DeserializeObject<Adscbdd>(respuesta.Resultado.ToString());
                if (respuesta.IsSuccess)
                {
                     
                    return View(respuesta.Resultado);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                var responseLog = new EntradaLog
                {
                    ExceptionTrace = ex.HResult.ToString() + " - " + ex.Message + " - " + ex.Source,
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
        public async Task<IActionResult> Edit(string id,Adscbdd adscbdd)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var respuestaActualizar = await apiServicio.SeleccionarAsync(adscbdd, new Uri(WebApp.BaseAddress),
                                                                  "api/BasesDatos/SeleccionarAdscBdd");
                    var respuesta = await apiServicio.EditarAsync(adscbdd, new Uri(WebApp.BaseAddress),
                                                                  "api/BasesDatos/EditarAdscbdd");

                    if (respuesta.IsSuccess)
                    {
                        var responseLog = new EntradaLog
                        {
                            ExceptionTrace = null,
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            ObjectPrevious = JsonConvert.SerializeObject(respuestaActualizar.Resultado),
                            ObjectNext = JsonConvert.SerializeObject(respuesta.Resultado),
                        };
                        await apiServicio.SalvarLog<entidades.Utils.Response>(HttpContext, responseLog);
                        return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }

                    TempData["Mensaje"] = $"{Mensaje.Error}|{respuesta.Message}";
                }
                return View(adscbdd);
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
                return this.Redireccionar($"{Mensaje.Error}|{ex.Message}");
            }
        }

        public async Task<IActionResult> Index(string mensaje)
        {
            var Listado = await apiServicio.Listar<Adscbdd>(new Uri(WebApp.BaseAddress), "api/BasesDatos/ListarBasesDatos");
            return View(Listado);
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (id != null)
                {
                    var response = await apiServicio.EliminarAsync(id,
                                                              new Uri(WebApp.BaseAddress),
                                                              "api/BasesDatos");
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
                    return this.Redireccionar($"{Mensaje.Aviso}|{response.Message}");
                }

                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Error}");
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
                return this.Redireccionar($"{Mensaje.Error}|{ex.Message}");
            }
        }
    }
}