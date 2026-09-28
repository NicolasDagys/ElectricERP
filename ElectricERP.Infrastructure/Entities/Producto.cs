using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Producto
{
    public int IdProducto { get; set; }

    public int IdProveedor { get; set; }

    public string Nombre { get; set; } = null!;

    public string CodigoSku { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string Categoria { get; set; } = null!;

    public string? FotoUrl { get; set; }

    public string UnidadMedida { get; set; } = null!;

    public virtual ICollection<DetalleInventario> DetalleInventarios { get; set; } = new List<DetalleInventario>();

    public virtual ICollection<DetalleTransferencia> DetalleTransferencia { get; set; } = new List<DetalleTransferencia>();

    public virtual ICollection<Etiqueta> Etiqueta { get; set; } = new List<Etiqueta>();

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    public virtual ICollection<SolicitudAjuste> SolicitudAjustes { get; set; } = new List<SolicitudAjuste>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
