using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class RecetaProducto
{
    public int RecetaId { get; set; }

    public int ProductoId { get; set; }

    public int MateriaPrimaId { get; set; }

    public decimal CantidadNecesaria { get; set; }

    public virtual MateriaPrima MateriaPrima { get; set; } = null!;

    public virtual ProductoTerminado Producto { get; set; } = null!;
}
