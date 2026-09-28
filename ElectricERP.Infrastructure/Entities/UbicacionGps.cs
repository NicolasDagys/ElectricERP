using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class UbicacionGps
{
    public int IdUbicacion { get; set; }

    public string IdTransportista { get; set; } = null!;

    public decimal Latitud { get; set; }

    public decimal Longitud { get; set; }

    public DateTime FechaHora { get; set; }

    public virtual ApplicationUser IdTransportistaNavigation { get; set; } = null!;
}
