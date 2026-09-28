using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Application.DTOs.Secciones
{
    public class SeccionDto
    {
        public int IdSeccion { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int IdAlmacen { get; set; }
        public string NombreAlmacen { get; set; } = string.Empty;
    }
}
