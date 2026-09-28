using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Stock
{
    public int IdStock { get; set; }

    public int IdProducto { get; set; }

    public int IdSeccion { get; set; }

    public int CantidadActual { get; set; }

    public int Minimo { get; set; }

    public int Maximo { get; set; }

    public DateTime UltimaActualizacion { get; set; }

    public virtual ICollection<Alerta> Alerta { get; set; } = new List<Alerta>();

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Seccion IdSeccionNavigation { get; set; } = null!;
}
