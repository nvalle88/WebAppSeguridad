
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bd.webappseguridad.servicios.Interfaces
{
  public  interface IAdscpasswServicio
    {
        Task<List<Adscpassw>> ListarAdscPasswAsync();
        Task<Response> CrearAsync(Adscpassw adscpassw);
        Task<Response> EliminarAsync(string id);
        Task<Response> EditarAsync(string id, Adscpassw adscpassw);
        Task<Response> SeleccionarAsync(string id);
    }
}
