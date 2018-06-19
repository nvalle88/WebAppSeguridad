﻿using System;
using System.Collections.Generic;
using System.Text;

namespace bd.webappseguridad.entidades.Utils
{
  public static class Mensaje
    {
        public static string Excepcion { get { return "Ha ocurrido una Excepción"; } }
        public static string NoExisteModulo { get { return "No se ha encontrado el Módulo"; } }
        public static string ExisteRegistro { get { return "Existe un registro de igual información"; } }
        public static string Satisfactorio { get { return "La acción se ha realizado satisfactoriamente"; } }
        public static string UsuarioSinConfirmar { get { return "Usuario sin Confirmar"; } }
        public static string RegistroNoEncontrado { get { return "El registro solicitado no se ha encontrado"; } }
        public static string ModeloInvalido { get { return "El Módelo es inválido"; } }
        public static string BorradoNoSatisfactorio { get { return "No es posible eliminar el registro, existen relaciones que dependen de él"; } }
        public static string Informacion { get { return "Información"; } }
        public static string Error { get { return "Error"; } }
        public static string Aviso { get { return "Aviso"; } }
    }
}
