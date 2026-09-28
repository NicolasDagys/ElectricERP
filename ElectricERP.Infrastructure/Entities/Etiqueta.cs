using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Etiqueta
{
    public int IdEtiqueta { get; set; }

    public int IdProducto { get; set; }

    public string CodigoQr { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
