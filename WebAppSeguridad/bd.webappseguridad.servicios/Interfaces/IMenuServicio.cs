using bd.webappseguridad.entidades.Negocio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bd.webappseguridad.servicios.Interfaces
{
   public interface IMenuServicio
    {
        Task<List<Adscmenu>> Listar(string usuario, string url);
        string ObtenerControlador(string Controlador);
        string ObtenerAccion(string Controlador);
    }
}
