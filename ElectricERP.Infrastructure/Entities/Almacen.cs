using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Almacen
{
    public int IdAlmacen { get; set; }

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public virtual ICollection<Seccion> Secciones { get; set; } = new List<Seccion>();

    public virtual ICollection<Transferencia> TransferenciaIdAlmacenDestinoNavigations { get; set; } = new List<Transferencia>();

    public virtual ICollection<Transferencia> TransferenciaIdAlmacenOrigenNavigations { get; set; } = new List<Transferencia>();
}
