using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Movimiento
{
    public long IdMovimiento { get; set; }

    public string IdUsuario { get; set; } = null!;

    public int IdProducto { get; set; }

    public int IdSeccion { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int CantidadUnidades { get; set; }

    public DateTime FechaHora { get; set; }

    public string Estado { get; set; } = null!;

    public int ValorAnterior { get; set; }

    public int ValorPosterior { get; set; }

    public string Resultado { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Seccion IdSeccionNavigation { get; set; } = null!;

    public virtual ApplicationUser IdUsuarioNavigation { get; set; } = null!;
}
