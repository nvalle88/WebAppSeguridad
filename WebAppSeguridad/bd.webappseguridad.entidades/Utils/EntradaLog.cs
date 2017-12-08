using System;
using System.Collections.Generic;
using System.Text;

namespace bd.webappseguridad.entidades.Utils
{
   public class EntradaLog
    {
        public string ExceptionTrace { get; set; }
        public string LogCategoryParametre { get; set; }
        public string LogLevelShortName { get; set; }
        public string ObjectPrevious { get; set; }
        public string ObjectNext { get; set; }
    }
}
