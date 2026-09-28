using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ElectricERP.Application.DTOs.Ajustes
{
    public class CrearSolicitudAjusteDto
    {
        public int IdProducto { get; set; }
        public string Tipo { get; set; } = string.Empty; // STOCK | INVENTARIO
        public string Motivo { get; set; } = string.Empty;
        public int CantidadSolicitada { get; set; }
    }

    public class ResolverSolicitudAjusteDto
    {
        public bool Aprobado { get; set; }
    }

    public class SolicitudAjusteDto
    {
        public int IdSolicitud { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public string NombreEmpleadoSolicitante { get; set; } = string.Empty;
        public string? NombreSupervisorResolutor { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public int CantidadSolicitada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaResolucion { get; set; }
    }
}