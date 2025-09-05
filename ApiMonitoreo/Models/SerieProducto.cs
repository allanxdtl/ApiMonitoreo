using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class SerieProducto
{
    public int SerieId { get; set; }

    public int ProduccionId { get; set; }

    public string NumeroSerie { get; set; } = null!;

    public virtual Produccion Produccion { get; set; } = null!;
}
