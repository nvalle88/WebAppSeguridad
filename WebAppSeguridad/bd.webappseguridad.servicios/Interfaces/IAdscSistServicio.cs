using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bd.webappseguridad.servicios.Interfaces
{
    public interface IAdscSistServicio
    {
        Task<List<Adscsist>> ListarAdscSistAsync();
        Task<Response> CrearAsync(Adscsist adscsist);
        Task<Response> EliminarAsync(string id);
        Task<Response> EditarAsync(string id, Adscsist adscsist);
        Task<Response> SeleccionarAsync(string id);
    }
}
