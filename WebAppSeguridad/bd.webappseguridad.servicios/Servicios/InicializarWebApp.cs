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
    public class InicializarWebApp
    {

        #region Methods

        public static async Task InicializarWeb(string baseAddreess)
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
            catch (Exception ex)
            {

            }

        }

        #endregion
    }
}
