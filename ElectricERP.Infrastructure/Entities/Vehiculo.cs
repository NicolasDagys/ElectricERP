using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Vehiculo
{
    public int IdVehiculo { get; set; }

    public string Matricula { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public int CapacidadCarga { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Transferencia> Transferencia { get; set; } = new List<Transferencia>();
}
