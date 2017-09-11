using bd.webappseguridad.entidades.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.ViewModels;

namespace bd.webappseguridad.servicios.Interfaces
{
   public interface IApiServicio
    {
        Task<Response> InsertarAsync<T>(T model,Uri baseAddress, string url );
        Task<Response> EliminarAsync(string id, Uri baseAddress, string url);
        Task<Response> EliminarAsync<T>(T model, Uri baseAddress, string url);
        Task<Response> EditarAsync<T>(string id, T model, Uri baseAddress, string url);
        Task<Response> EditarAsync<T>(T model, Uri baseAddress, string url);
        Task<T> SeleccionarAsync<T>(string id, Uri baseAddress, string url) where T : class;
        Task<Response> SeleccionarAsync<T>(T model,Uri baseAddress, string url) where T : class;
        Task<DetalleMenu> DetalleMenuAsync<T>(T model, Uri baseAddress, string url) where T : class;
        Task<List<T>> Listar<T>(Uri baseAddress, string url) where T :class;
        Task<List<Adscbdd>> Listar<T>(T model, Uri baseAddress, string url) where T : class;
        Task<List<Adscmenu>> ListarPadresPorSistema<T>(T model, Uri baseAddress, string url) where T : class;
        Task<List<Adscgrp>> ListarGrupoPorBdd<T>(T model, Uri baseAddress, string url) where T : class;
        Task<List<Adscmenu>> ListarAplicacionPorSistema<T>(T model, Uri baseAddress, string url) where T : class;

    }
}
