using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Entities;

public partial class Alerta
{
    public int IdAlerta { get; set; }

    public int IdStock { get; set; }

    public string TipoAlerta { get; set; } = null!;

    public DateTime FechaHora { get; set; }

    public bool Estado { get; set; }

    public virtual Stock IdStockNavigation { get; set; } = null!;
}
