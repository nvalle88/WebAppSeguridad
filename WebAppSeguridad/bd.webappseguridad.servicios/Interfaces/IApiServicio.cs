using bd.webappseguridad.entidades.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using bd.webappseguridad.entidades.Negocio;
using bd.webappseguridad.entidades.ViewModels;
using bd.log.guardar.ObjectTranfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;


namespace bd.webappseguridad.servicios.Interfaces
{
   public interface IApiServicio
    {
        Task<Response> SalvarLog<T>(HttpContext context, EntradaLog model);
        Task<Response> InsertarAsync<T>(T model,Uri baseAddress, string url );
        Task<Response> EliminarAsync(string id, Uri baseAddress, string url);
        Task<Response> EliminarAsync<T>(T model, Uri baseAddress, string url);
        Task<Response> EditarAsync<T>(string id, T model, Uri baseAddress, string url);
        Task<Response> EditarAsync<T>(T model, Uri baseAddress, string url);
        Task<Response> EditarAsync<T>(object model, Uri baseAddress, string url);
        Task<T> ObtenerElementoAsync1<T>(object model, Uri baseAddress, string url) where T : class;
        Task<T> SeleccionarAsync<T>(string id, Uri baseAddress, string url) where T : class;
        Task<entidades.Utils.Response> SeleccionarAsync<T>(T model,Uri baseAddress, string url) where T : class;
        Task<DetalleMenu> DetalleMenuAsync<T>(T model, Uri baseAddress, string url) where T : class;
        Task<List<T>> Listar<T>(Uri baseAddress, string url) where T :class;
        Task<List<T>> Listar<T>(object model, Uri baseAddress, string url) where T : class;
        Task<List<Adscbdd>> Listar<T>(T model, Uri baseAddress, string url) where T : class;
        Task<List<Adscmenu>> ListarPadresPorSistema<T>(T model, Uri baseAddress, string url) where T : class;
        Task<List<Adscgrp>> ListarGrupoPorBdd<T>(T model, Uri baseAddress, string url) where T : class;
        Task<List<Adscmenu>> ListarAplicacionPorSistema<T>(T model, Uri baseAddress, string url) where T : class;

    }
}
