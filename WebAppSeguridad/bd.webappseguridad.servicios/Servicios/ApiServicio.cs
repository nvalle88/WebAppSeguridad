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
using bd.log.guardar.Utiles;

namespace bd.webappseguridad.servicios.Servicios
{
    public class ApiServicio :Controller, IApiServicio
    {
        public async Task<entidades.Utils.Response> InsertarAsync<T>(T model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}{1}", baseAddress, url);
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

        public async Task<entidades.Utils.Response> EliminarAsync<T>(T model, Uri baseAddress, string url)
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
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<entidades.Utils.Response> EditarAsync<T>(object model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}{1}", baseAddress, url);

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

        public async Task<List<T>> Listar<T>(object model, Uri baseAddress, string url) where T : class
        {

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}{1}", baseAddress, url);

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

        public async Task<entidades.Utils.Response> EliminarAsync(string id, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    url = string.Format("{0}/{1}", url, id);
                    var uri = string.Format("{0}{1}", baseAddress, url);
                    var response = await client.DeleteAsync(new Uri(uri));

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

        public async Task<entidades.Utils.Response> EditarAsync<T>(T model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}{1}", baseAddress, url);
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
                    var uri = string.Format("{0}{1}", baseAddress, url);

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

                    var uri = string.Format("{0}{1}", baseAddress, url);
                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<List<Adscmenu>>(resultado);
                    return respuesta;
                }
            }

            catch (Exception ex)
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

                    var uri = string.Format("{0}{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<List<Adscgrp>>(resultado);
                    return respuesta;
                }
            }

            catch (Exception ex)
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

                    var uri = string.Format("{0}{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<List<Adscmenu>>(resultado);
                    return respuesta;
                }
            }

            catch (Exception ex)
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

                    var uri = string.Format("{0}{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<List<Adscbdd>>(resultado);
                    return respuesta;
                }
            }

            catch (Exception ex)
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
                    var uri = string.Format("{0}{1}", baseAddress, url);
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

                    var uri = string.Format("{0}{1}", baseAddress, url);

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

                    var uri = string.Format("{0}{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<T>(resultado);
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<entidades.Utils.Response> SeleccionarAsync<T>(T model, Uri baseAddress, string url) where T : class
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<entidades.Utils.Response>(resultado);
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
                    var uri = string.Format("{0}{1}", baseAddress, url);
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

        public async Task<log.guardar.Utiles.Response> SalvarLog<T>(HttpContext context, EntradaLog model)
        {
            var menuRespuesta = await ObtenerElementoAsync1<log.guardar.Utiles.Response>(new ModuloAplicacion { Path = context.Request.Path }, new Uri(WebApp.BaseAddress), "api/Adscmenus/GetMenuPadre");
            var menu = JsonConvert.DeserializeObject<Adscmenu>(menuRespuesta.Resultado.ToString());

            var claim = context.User.Identities.Where(x => x.NameClaimType == ClaimTypes.Name).FirstOrDefault();
            var NombreUsuario = claim.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

            var Log = new LogEntryTranfer
            {
                ApplicationName = WebApp.NombreAplicacion,
                EntityID = menu.AdmeAplicacion,
                ExceptionTrace = model.ExceptionTrace,
                LogCategoryParametre = model.LogCategoryParametre,
                LogLevelShortName = model.LogLevelShortName,
                Message=context.Request.Path,
                ObjectNext=model.ObjectNext,
                ObjectPrevious=model.ObjectPrevious,
                UserName=NombreUsuario,
            };
            var responseLog = await GuardarLogService.SaveLogEntry(Log);
            return new log.guardar.Utiles.Response { IsSuccess=responseLog.IsSuccess};
        }
    }
}
