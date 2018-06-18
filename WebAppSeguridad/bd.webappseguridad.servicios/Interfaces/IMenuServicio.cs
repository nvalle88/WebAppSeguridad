using bd.webappseguridad.entidades.Negocio;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bd.webappseguridad.servicios.Interfaces
{
    public interface IMenuServicio
    {
        Task<List<Adscmenu>> Listar(string usuario, string url);
        string ObtenerControlador(string Controlador);
        string ObtenerAccion(string Controlador);
        List<string> ObtenerAccionesDiccionario(string admeControlador);
        IHtmlContent CrearMenu(IHtmlHelper helper, IUrlHelper url, List<Adscmenu> menuItems, int niveles);
    }
}
