using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Application.DTOs.Stock
{
    public class ActualizarLimitesDto
    {
        public int Minimo { get; set; }
        public int Maximo { get; set; }
    }
}