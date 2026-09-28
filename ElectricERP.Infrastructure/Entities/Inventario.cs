using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Inventario
{
    public int IdInventario { get; set; }

    public int IdSeccion { get; set; }

    public DateTime FechaAlta { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? FechaCierre { get; set; }

    public virtual Seccion IdSeccionNavigation { get; set; } = null!;

    public virtual ICollection<DetalleInventario> DetalleInventarios { get; set; } = new List<DetalleInventario>();
}