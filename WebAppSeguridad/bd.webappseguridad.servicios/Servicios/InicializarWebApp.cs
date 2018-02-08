using System;
using System.Net.Http;
using System.Threading.Tasks;

using bd.webappseguridad.entidades.Utils;
using bd.log.guardar.Inicializar;
using Newtonsoft.Json;
using bd.webappseguridad.entidades.Negocio;
using Microsoft.Extensions.Configuration;

namespace bd.webappseguridad.servicios.Servicios
{


    /// <summary>
    /// Está clase es la encargada de inicializar variables necesarias para el uso de la aplicación 
    /// estas variables son los host donde se encuentran los servicios web 
    /// Ejemplo:WebApp.BaseAddressSeguridad es el host donde se encuentran los servicios de Seguridad.
    ///  AppGuardarLog.BaseAddress es el host donde se encuentran los servicios de Log.
    /// </summary>
    /// 
    public class InicializarWebApp
    {

        #region Methods


        /// <summary>
        /// Inicializar el host de Seguridad para poder consumir los servicios de seguridad
        /// </summary>
        /// <param name="baseAddreess">Host donde se encuentra el servicio de seguridad (appsetting.json)</param>
        /// <returns></returns>
        /// 

        public static void InicializarWeb(string baseAddreess)
        {
            try
            {
                WebApp.BaseAddress = baseAddreess;
               // WebApp.BaseAddress = "http://localhost:53317";
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// Inicializar en la variable AppGuardarLog.BaseAddress el host de los Log 
        /// para poder consumir los servicios de log
        /// </summary>
        /// <param name="id">Nombre del sistema de log igual que como esté en la base de datos</param>
        /// <param name="baseAddress">Host donde se encuentra el servicio de seguridad (appsetting.json)</param>
        /// <returns></returns>

        public static async Task InicializarLogEntry(string id,Uri baseAddress)
        {
           
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    var url = string.Format("{0}/{1}", "/api/Adscsists", id);
                    var uri = string.Format("{0}/{1}", baseAddress, url);
                    var respuesta = await client.GetAsync(new Uri(uri));

                    var resultado = await respuesta.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Response>(resultado);
                    var sistema = JsonConvert.DeserializeObject<Adscsist>(response.Resultado.ToString());
                    AppGuardarLog.BaseAddress= sistema.AdstHost;
                    //AppGuardarLog.BaseAddress = "http://localhost:50257";
                }
            }
            catch (Exception )
            {
                throw;
            }

        }

        #endregion
    }
}
