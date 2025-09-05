using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class LoteMateriaPrima
{
    public int LoteId { get; set; }

    public int MateriaPrimaId { get; set; }

    public DateOnly FechaEntrada { get; set; }

    public decimal CantidadInicial { get; set; }

    public decimal CantidadDisponible { get; set; }

    public virtual ICollection<ConsumoMateriaPrima> ConsumoMateriaPrimas { get; set; } = new List<ConsumoMateriaPrima>();

    public virtual MateriaPrima MateriaPrima { get; set; } = null!;
}
