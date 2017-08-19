using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.servicios.Interfaces;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;

namespace bd.webappcompartido.servicios.Servicios
{
    public class BaseDatosServicio : IBaseDatosServicio
    {
        #region Atributos




        #endregion

        #region Servicios
        private readonly IApiServicio apiservicio;
        #endregion

        #region Constructores

        public BaseDatosServicio(IApiServicio apiservicio)
        {
            this.apiservicio = apiservicio;

        }

        #endregion

        #region Metodos

        public async Task<Response> CrearAsync(Adscbdd adscbdd)
        {
            Response response = new Response();
            try
            {
                response = await apiservicio.InsertarAsync(adscbdd,
                                                             new Uri(WebApp.BaseAddress),
                                                             "/api/BasesDatos/InsertarBaseDatos");
                if (response.IsSuccess)
                {

                    var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                        ExceptionTrace = null,
                        Message = "Se ha creado una base de datos",
                        UserName = "Usuario 1",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        EntityID = string.Format("{0} {1}", "Base de Datos:", adscbdd.AdbdBdd),
                    });
                }

                return response;

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Creando Base de Datos",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });

                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<Response> EliminarAsync(string id)
        {
            Response response = new Response();
            try
            {
                response = await apiservicio.EliminarAsync(id,
                                                              new Uri(WebApp.BaseAddress),
                                                              "/api/BasesDatos");
                if (response.IsSuccess)
                {
                    await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                        EntityID = string.Format("{0} : {1}", "BaseDatos", id),
                        Message = "Registro eliminado",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        UserName = "Usuario APP Seguridad"
                    });
                }
                return response;
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
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<Response> EditarAsync(string id, Adscbdd adscbdd)
        {
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    response = await apiservicio.EditarAsync(id, adscbdd, new Uri(WebApp.BaseAddress),
                                                                 "/api/BasesDatos");

                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1}", "Base de Datos", id),
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            Message = "Se ha actualizado un registro",
                            UserName = "Usuario 1"
                        });
                    }

                }
                return response;
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Editando una base de datos",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<Response> SeleccionarAsync(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var respuesta = await apiservicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                                  "/api/BasesDatos");


                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscbdd>(respuesta.Resultado.ToString());
                    if (respuesta.IsSuccess)
                    {
                        return respuesta;
                    }

                }

                return new Response
                {
                    IsSuccess = false,
                    Message = "Id no válido",
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<List<Adscbdd>> ListarBaseDatosAsync()
        {
            var lista = new List<Adscbdd>();
            try
            {

                lista = await apiservicio.Listar<Adscbdd>(new Uri(WebApp.BaseAddress), "/api/BasesDatos/ListarBasesDatos");
                return lista;
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                    Message = "Listando Bases de datos",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.NetActivity),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP Seguridad"
                });
                return lista=null;
            }
        }

        #endregion
    }
}
