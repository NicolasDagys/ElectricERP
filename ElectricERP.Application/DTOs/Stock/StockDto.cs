using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Application.DTOs.Stock
{
    public class StockDto
    {
        public int IdStock { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public int IdSeccion { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public string NombreAlmacen { get; set; } = string.Empty;
        public int CantidadActual { get; set; }
        public int Minimo { get; set; }
        public int Maximo { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public string EstadoAlerta { get; set; } = "Normal"; // Normal | Minimo | Maximo
    }
}
