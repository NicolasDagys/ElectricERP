using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class DetalleInventario
{
    public int IdDetalleInventario { get; set; }

    public int IdInventario { get; set; }

    public int IdProducto { get; set; }

    public int CantidadInventario { get; set; }

    public virtual Inventario IdInventarioNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
