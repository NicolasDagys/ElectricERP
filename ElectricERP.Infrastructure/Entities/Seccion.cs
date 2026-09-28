using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Seccion
{
    public int IdSeccion { get; set; }

    public int IdAlmacen { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual Almacen IdAlmacenNavigation { get; set; } = null!;

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<Inventario> Inventarios { get; set; } = null!;
}
