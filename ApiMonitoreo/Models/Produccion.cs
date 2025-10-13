using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class Produccion
{
    public int ProduccionId { get; set; }

    public int ProductoId { get; set; }

    public int CantidadProducida { get; set; }

    public DateOnly FechaProduccion { get; set; }

    public int? OrdenId { get; set; }

    public virtual ICollection<ConsumoMateriaPrima> ConsumoMateriaPrimas { get; set; } = new List<ConsumoMateriaPrima>();

    public virtual Orden? Orden { get; set; }

    public virtual ProductoTerminado Producto { get; set; } = null!;

    public virtual ICollection<SerieProducto> SerieProductos { get; set; } = new List<SerieProducto>();
}
