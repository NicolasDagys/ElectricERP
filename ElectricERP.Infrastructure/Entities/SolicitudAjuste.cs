using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class SolicitudAjuste
{
    public int IdSolicitud { get; set; }

    public int IdProducto { get; set; }

    public string IdEmpleadoSolicitante { get; set; } = null!;

    public string? IdSupervisorResolutor { get; set; }

    public string Tipo { get; set; } = null!;

    public string Motivo { get; set; } = null!;

    public int CantidadSolicitada { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaSolicitud { get; set; }

    public DateTime? FechaResolucion { get; set; }

    public virtual ApplicationUser IdEmpleadoSolicitanteNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual ApplicationUser? IdSupervisorResolutorNavigation { get; set; }

}
