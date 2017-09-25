using bd.webappseguridad.entidades.Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace bd.webappseguridad.entidades.ViewModels
{
  public  class MenusGrupo
    {
        [Display(Name = "Base de datos")]
        public string Adgrbdd { get; set; }

        [Display(Name = "Grupo")]
        public string Adgrgrupo { get; set; }

        [Display(Name = "Sistema")]
        public string AdmeSistema { get; set; }

        [Display(Name = "Aplicación")]
        public string AdmeAplicacion { get; set; }

        public List<Adscmenu> listaMenus { get; set; }
    }
}
