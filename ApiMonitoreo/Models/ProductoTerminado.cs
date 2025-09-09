using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class ProductoTerminado
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = null!;

    public int ExistenciaActual { get; set; }

    public virtual ICollection<Produccion> Produccions { get; set; } = new List<Produccion>();

    public virtual ICollection<RecetaProducto> RecetaProductos { get; set; } = new List<RecetaProducto>();
}
