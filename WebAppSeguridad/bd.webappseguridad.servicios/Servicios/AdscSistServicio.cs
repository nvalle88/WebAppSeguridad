using bd.log.guardar.Enumeradores;
using bd.log.guardar.ObjectTranfer;
using bd.log.guardar.Servicios;
using bd.webappseguridad.entidades.Enumeradores;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.servicios.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bd.webappseguridad.servicios.Servicios
{
   public class AdscSistServicio: IAdscSistServicio
    {
        #region Atributos




        #endregion

        #region Servicios
        private readonly IApiServicio apiservicio;
        #endregion

        #region Constructores

        public AdscSistServicio(IApiServicio apiservicio)
        {
            this.apiservicio = apiservicio;

        }

        #endregion

        #region Metodos

        public async Task<Response> CrearAsync(Adscsist adscsist)
        {
            Response response = new Response();
            try
            {
                response = await apiservicio.InsertarAsync(adscsist,
                                                             new Uri(WebApp.BaseAddress),
                                                             "/api/Adscsists/InsertarAdscSist");
                if (response.IsSuccess)
                {

                    var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                        ExceptionTrace = null,
                        Message = "Se ha creado un sistema",
                        UserName = "Usuario 1",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        EntityID = string.Format("{0} {1}", "Sistema:", adscsist.AdstSistema),
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
                                                              "/api/Adscsists");
                if (response.IsSuccess)
                {
                    await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                        EntityID = string.Format("{0} : {1}", "Sistema", id),
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

        public async Task<Response> EditarAsync(string id, Adscsist adscsist)
        {
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    response = await apiservicio.EditarAsync(id, adscsist, new Uri(WebApp.BaseAddress),
                                                                 "/api/Adscsists");

                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppSeguridad),
                            EntityID = string.Format("{0} : {1}", "Sistema", id),
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            Message = "Se ha actualizado un registro sistema",
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
                                                                  "/api/Adscsists");


                    respuesta.Resultado = JsonConvert.DeserializeObject<Adscsist>(respuesta.Resultado.ToString());
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

        public async Task<List<Adscsist>> ListarAdscSistAsync()
        {
            var lista = new List<Adscsist>();
            try
            {

                lista = await apiservicio.Listar<Adscsist>(new Uri(WebApp.BaseAddress)
                                                                    ,"/api/Adscsists/ListarAdscSistema");
                return lista;
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
                return lista = null;
            }
        }

        #endregion
    }
}
