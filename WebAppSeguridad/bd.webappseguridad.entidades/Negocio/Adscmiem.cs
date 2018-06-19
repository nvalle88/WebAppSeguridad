﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bd.webappseguridad.entidades.Negocio
{
    public partial class Adscmiem
    {
        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Usuario")]
        public string AdmiEmpleado { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Grupo")]
        public string AdmiGrupo { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Base de datos")]
        public string AdmiBdd { get; set; }


        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Administrador Total")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El {0} no puede tener más de {1} y menos de {2}")]
        public string AdmiTotal { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Código de empleado")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El {0} no puede tener más de {1} y menos de {2}")]
        public string AdmiCodigoEmpleado { get; set; }

        public virtual Adscgrp Admi { get; set; }
    }
}
