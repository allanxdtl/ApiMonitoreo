using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class ProductoTerminado
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = null!;

    public int ExistenciaActual { get; set; }

    public decimal? Precio { get; set; }

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    public virtual ICollection<Produccion> Produccions { get; set; } = new List<Produccion>();

    public virtual ICollection<PruebaProducto> PruebaProductos { get; set; } = new List<PruebaProducto>();

    public virtual ICollection<RecetaProducto> RecetaProductos { get; set; } = new List<RecetaProducto>();
}
