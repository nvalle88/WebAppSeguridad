using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.Utils;
using bd.webappseguridad.servicios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bd.webappseguridad.servicios.Servicios
{
       

    public class InicializacionServico : IInicializacionServico
    {
        private readonly IAdscSistServicio adscSistServicio;

        public InicializacionServico(IAdscSistServicio adscSistServicio)
        {
            this.adscSistServicio = adscSistServicio;
        }

        public  async void InicializacionAsync()
        {
            var response = await adscSistServicio.SeleccionarAsync("swSeguridad");
            var sistema = (Adscsist)response.Resultado;
            WebApp.BaseAddress = sistema.AdstHost;
        }
    }
}
