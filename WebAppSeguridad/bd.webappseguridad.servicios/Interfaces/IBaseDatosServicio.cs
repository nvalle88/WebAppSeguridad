using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bd.webappseguridad.servicios.Interfaces
{
    public interface  IBaseDatosServicio
    {
        Task<List<Adscbdd>> ListarBaseDatosAsync();
        Task<Response> CrearAsync(Adscbdd adscbdd);
        Task<Response> EliminarAsync(string id);
        Task<Response> EditarAsync(string id,Adscbdd adscbdd);
        Task<Response> SeleccionarAsync(string id);
    }
}
