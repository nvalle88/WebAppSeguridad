using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.servicios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bd.webappseguridad.servicios.Servicios
{
   public class MenuServicio:IMenuServicio
    {


        public string ObtenerControlador(string Controlador)
        {
            if (Controlador!=null)
            {
                var matriz = Controlador.Split('/');
                var salida = matriz[1];
                return salida;
            }

            return "";
           
        }

        public string ObtenerAccion(string Controlador)
        {
            if (Controlador != null)
            {
                var matriz = Controlador.Split('/');
                var salida = matriz[2];
                return salida;
            }

            return "";

        }

        public async Task<List<Adscmenu>> Listar(string usuario,string url)
        {
            var usuarioSistema = new UsuarioSistema
            {
                Sistema = WebApp.NombreAplicacion,
                Usuario =usuario,
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(usuarioSistema);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", WebApp.BaseAddress, url);

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
    }
}
