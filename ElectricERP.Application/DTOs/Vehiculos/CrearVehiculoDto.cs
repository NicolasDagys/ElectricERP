using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Application.DTOs.Vehiculos
{
    public class CrearVehiculoDto
    {
        public string Matricula { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CapacidadCarga { get; set; }
        public bool Activo { get; set; } = true;
    }
}
