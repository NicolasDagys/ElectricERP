using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Application.DTOs.Stock
{
    public class CrearStockDto
    {
        public int IdProducto { get; set; }
        public int IdSeccion { get; set; }
        public int CantidadActual { get; set; }
        public int Minimo { get; set; }
        public int Maximo { get; set; }
    }
}
