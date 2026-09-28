using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Application.DTOs.Transferencias
{
    public class RegistrarUbicacionDto
    {
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
    }

    public class UbicacionDto
    {
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public DateTime FechaHora { get; set; }
    }
}