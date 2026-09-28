using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class DetalleTransferencia
{
    public int IdDetalleTransferencia { get; set; }

    public int IdTransferencia { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Transferencia IdTransferenciaNavigation { get; set; } = null!;
}
