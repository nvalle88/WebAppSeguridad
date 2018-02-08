using bd.webappseguridad.servicios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using bd.webappseguridad.entidades.Utils;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.ViewModels;
using bd.log.guardar.ObjectTranfer;
using bd.log.guardar.Servicios;
using bd.webappseguridad.entidades.ModeloTransferencia;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using bd.log.guardar.Enumeradores;

namespace bd.webappseguridad.servicios.Servicios
{
    /// <summary>
    /// Clase genérica para consumir los servicios la cuál hereda de una interfaz para poder realizar la 
    /// injección de dependencia en los controladores MVC
    /// en esta clase hay varios métodos sobrecargados para mejorar la experiencia del desarrollador 
    /// y tener cubiertas gran parte de las necesidades.
    /// Hay varios aspectos que hay que explicar como:
    /// <typeparam name="T">Genérico que acepta cualquier tipo de objeto </typeparam>
    /// <param name="model">acepta cualquier tipo de objeto</param>
    /// <param name="baseAddress">¿Cuál es el Host donde encuentra el servicio web a consummir?</param>
    /// <param name="url">Recurso que deseamos consumir</param>
    /// Response: es una clase que tiene una variable bool que determina si lo que se solicito es satisfactorio o no
    /// un String Mnesaje: para si se desea obtener algún mensaje desde el servicio.
    /// y un objeto de tipo object para devolver del servicio el objeto que se desee
    /// Nota:Tener en cuenta que esta clase no realiza ninguna acción sobre la base de datos.
    /// Solo envía información a  los servicios Web.
    /// En el caso cunatro introducimos un Object como parámetro tenemos que tener en cuenta que 
    /// en el servicio web que estamos consumiento debe tener un objeto similar al que enviamos para 
    /// poder deserializarlo.
    /// Ejemplo:
    /// MVC                                            ServicioWeb
    /// Obteto 
    /// Animal:Tamaño:int,Nombre:string                Animal:Color,Tamaño:int,Nombre:string
    /// El nombre del objeto es indiferente,
    /// pero sus atributos si deben ser iguales
    /// es decir que en MVC puedo tener un objeto de Tipo Perro:Tamaño:int,Nombre:string y si lo envío al servicio 
    /// y el que captura es Animal es indiferente porque se deserializa al los nombres de los atributos y su tipo de datos
    /// y no al nombre del objeto.
    /// 
    /// </summary>
    public class ApiServicio :Controller, IApiServicio
    {
        public async Task<Response> InsertarAsync<T>(T model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);
                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<entidades.Utils.Response>(resultado);
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                return new entidades.Utils.Response
                {
                    IsSuccess = true,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response> EliminarAsync<T>(T model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);
                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<Response>(resultado);
                    return respuesta;

                }
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

        public async Task<Response> EditarAsync<T>(object model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<Response>(resultado);
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = ex.Message,
                };
            }
        }

        public async Task<List<T>> Listar<T>(object model, Uri baseAddress, string url) where T : class
        {

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<List<T>>(resultado);
                    return respuesta;

                }

            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<Response> EliminarAsync(string id, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    url = string.Format("{0}/{1}", url, id);
                    var uri = string.Format("{0}/{1}", baseAddress, url);
                    var response = await client.DeleteAsync(new Uri(uri));

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<Response>(resultado);
                    return respuesta;

                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = ex.Message,
                };
            }
        }

        public async Task<entidades.Utils.Response> EditarAsync<T>(T model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);
                    var response = await client.PutAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<entidades.Utils.Response>(resultado);
                    return respuesta;

                }
            }
            catch (Exception ex)
            {
                return new entidades.Utils.Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<entidades.Utils.Response> EditarAsync<T>(string id, T model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    url = string.Format("{0}/{1}", url, id);
                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PutAsync(new Uri(uri), content);
                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<entidades.Utils.Response>(resultado);
                    return respuesta;

                }
            }
            catch (Exception ex)
            {
                return new entidades.Utils.Response
                {
                    IsSuccess = true,
                    Message = ex.Message,
                };
            }
        }

        public async Task<List<Adscmenu>> ListarAplicacionPorSistema<T>(T model, Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);
                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<List<Adscmenu>>(resultado);
                    return respuesta;
                }
            }

            catch (Exception)
            {
                return null;
            }

        }

        public async Task<List<Adscgrp>> ListarGrupoPorBdd<T>(T model, Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<List<Adscgrp>>(resultado);
                    return respuesta;
                }
            }

            catch (Exception )
            {
                return null;
            }

        }

        public async Task<List<Adscmenu>> ListarPadresPorSistema<T>(T model, Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<List<Adscmenu>>(resultado);
                    return respuesta;
                }
            }

            catch (Exception )
            {
                return null;
            }

        }

        public async Task<List<Adscbdd>> Listar<T>(T model, Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<List<Adscbdd>>(resultado);
                    return respuesta;
                }
            }

            catch (Exception )
            {
                return null;
            }

        }

        public async Task<List<T>> Listar<T>(Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var uri = string.Format("{0}/{1}", baseAddress, url);
                    var respuesta = await client.GetAsync(new Uri(uri));

                    var resultado = await respuesta.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<List<T>>(resultado);
                    return response;
                }
            }

            catch (Exception)
            {
                return null;
            }

        }

        public async Task<DetalleMenu> DetalleMenuAsync<T>(T model, Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<DetalleMenu>(resultado);
                    return respuesta;
                }
            }
            catch (Exception)
            {
                return new DetalleMenu();
            }

        }

        public async Task<T> ObtenerElementoAsync1<T>(object model, Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<T>(resultado);
                    return respuesta;
                }
            }
            catch (Exception )
            {
                throw;
            }
        }

        public async Task<Response> SeleccionarAsync<T>(T model, Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<Response>(resultado);
                    return respuesta;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        public async Task<T> SeleccionarAsync<T>(string id, Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    url = string.Format("{0}/{1}", url, id);
                    var uri = string.Format("{0}/{1}", baseAddress, url);
                    var respuesta = await client.GetAsync(new Uri(uri));

                    var resultado = await respuesta.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<T>(resultado);
                    return response;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        private async Task<bool> SalvarLog(LogEntryTranfer logEntryTranfer)
        {
            var responseLog = await GuardarLogService.SaveLogEntry(logEntryTranfer);
            if (responseLog.IsSuccess)
            {
                return true;
            }

            return false;
        }

        public async Task<Response> SalvarLog<T>(HttpContext context, EntradaLog model)
        {


            var NombreUsuario = "";
            try
            {
                var claim = context.User.Identities.Where(x => x.NameClaimType == ClaimTypes.Name).FirstOrDefault();
                NombreUsuario = claim.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

                var menuRespuesta = await ObtenerElementoAsync1<log.guardar.Utiles.Response>(new ModuloAplicacion { Path = context.Request.Path, NombreAplicacion = WebApp.NombreAplicacion }, new Uri(WebApp.BaseAddress), "api/Adscmenus/GetMenuPadre");
                var menu = JsonConvert.DeserializeObject<Adscmenu>(menuRespuesta.Resultado.ToString());
               
                var Log = new LogEntryTranfer
                {
                    ApplicationName = WebApp.NombreAplicacion,
                    EntityID = menu.AdmeAplicacion,
                    ExceptionTrace = model.ExceptionTrace,
                    LogCategoryParametre = model.LogCategoryParametre,
                    LogLevelShortName = model.LogLevelShortName,
                    Message = context.Request.Path,
                    ObjectNext = model.ObjectNext,
                    ObjectPrevious = model.ObjectPrevious,
                    UserName = NombreUsuario,
                };
                var responseLog = await GuardarLogService.SaveLogEntry(Log);
                return new Response { IsSuccess = responseLog.IsSuccess };
            }
            catch (Exception ex)
            {
                var Log = new LogEntryTranfer
                {
                    ApplicationName = WebApp.NombreAplicacion,
                    EntityID = Mensaje.NoExisteModulo,
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = model.LogCategoryParametre,
                    LogLevelShortName = model.LogLevelShortName,
                    Message = context.Request.Path,
                    ObjectNext = model.ObjectNext,
                    ObjectPrevious = model.ObjectPrevious,
                    UserName = NombreUsuario,
                };
              var resultado= await SalvarLog(Log);
                return new Response { IsSuccess = resultado };
            }

        }
    }
}
