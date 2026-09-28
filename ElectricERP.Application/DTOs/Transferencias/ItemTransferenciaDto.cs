namespace ElectricERP.Application.DTOs.Transferencias
{
    public class ItemTransferenciaDto
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }

    public class CrearTransferenciaDto
    {
        public int IdAlmacenOrigen { get; set; }
        public int IdAlmacenDestino { get; set; }
        public List<ItemTransferenciaDto> Productos { get; set; } = new();
    }

    public class AutorizarTransferenciaDto
    {
        public bool Aprobado { get; set; }
        public string? Motivo { get; set; }
    }

    public class AsignarTransferenciaDto
    {
        public int IdVehiculo { get; set; }
        public string IdTransportista { get; set; } = string.Empty;
    }

    public class DetalleTransferenciaItemDto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class SimularUbicacionDto
    {
        public string IdTransportista { get; set; } = string.Empty;
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
    }

    public class TransferenciaDto
    {
        public int IdTransferencia { get; set; }
        public int IdAlmacenOrigen { get; set; }
        public string NombreAlmacenOrigen { get; set; } = string.Empty;
        public int IdAlmacenDestino { get; set; }
        public string NombreAlmacenDestino { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string NombreSupervisorSolicitante { get; set; } = string.Empty;
        public string? NombreAdministradorAutorizador { get; set; }
        public string? IdTransportista { get; set; }
        public string? NombreTransportista { get; set; }
        public string? Matricula { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public List<DetalleTransferenciaItemDto> Detalles { get; set; } = new();
    }
}