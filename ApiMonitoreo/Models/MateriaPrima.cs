using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class MateriaPrima
{
    public int MateriaPrimaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string UnidadMedida { get; set; } = null!;

    public virtual ICollection<ConsumoMateriaPrima> ConsumoMateriaPrimas { get; set; } = new List<ConsumoMateriaPrima>();

    public virtual ICollection<LoteMateriaPrima> LoteMateriaPrimas { get; set; } = new List<LoteMateriaPrima>();

    public virtual ICollection<RecetaProducto> RecetaProductos { get; set; } = new List<RecetaProducto>();
}
