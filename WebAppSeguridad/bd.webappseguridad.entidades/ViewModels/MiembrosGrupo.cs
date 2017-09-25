using bd.webappseguridad.entidades.Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace bd.webappseguridad.entidades.ViewModels
{
    public class MiembrosGrupo
    {
        [Display(Name = "Base de datos")]
        public string Adgrbdd { get; set; }

        [Display(Name = "Grupo")]
        public string Adgrgrupo { get; set; }

        public List<Adscmiem> ListaMiembros {get;set;}
       
    }
}
