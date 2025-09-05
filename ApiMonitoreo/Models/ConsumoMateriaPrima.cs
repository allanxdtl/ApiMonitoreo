using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class ConsumoMateriaPrima
{
    public int ConsumoId { get; set; }

    public int ProduccionId { get; set; }

    public int LoteId { get; set; }

    public int MateriaPrimaId { get; set; }

    public decimal CantidadUsada { get; set; }

    public virtual LoteMateriaPrima Lote { get; set; } = null!;

    public virtual MateriaPrima MateriaPrima { get; set; } = null!;

    public virtual Produccion Produccion { get; set; } = null!;
}
