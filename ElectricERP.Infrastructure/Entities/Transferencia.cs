using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Transferencia
{
    public int IdTransferencia { get; set; }

    public string IdSupervisorSolicitante { get; set; } = null!;

    public string? IdAdministradorAutorizador { get; set; }

    public string? IdTransportista { get; set; }

    public int? IdVehiculo { get; set; }

    public int IdAlmacenOrigen { get; set; }

    public int IdAlmacenDestino { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaSolicitud { get; set; }

    public DateTime? FechaAutorizacion { get; set; }

    public DateTime? FechaEjecucion { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public virtual ICollection<DetalleTransferencia> DetalleTransferencia { get; set; } = new List<DetalleTransferencia>();

    public virtual ApplicationUser? IdAdministradorAutorizadorNavigation { get; set; }

    public virtual Almacen IdAlmacenDestinoNavigation { get; set; } = null!;

    public virtual Almacen IdAlmacenOrigenNavigation { get; set; } = null!;

    public virtual ApplicationUser IdSupervisorSolicitanteNavigation { get; set; }= null!;

    public virtual ApplicationUser? IdTransportistaNavigation { get; set; }

    public virtual Vehiculo? IdVehiculoNavigation { get; set; }
}
