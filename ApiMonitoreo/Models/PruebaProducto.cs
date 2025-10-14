using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class PruebaProducto
{
    public int IdpruebaProducto { get; set; }

    public int ProductoId { get; set; }

    public int Idprueba { get; set; }

    public decimal ValorEsperado { get; set; }

    public decimal Tolerancia { get; set; }

    public string? Comentario { get; set; }

    public virtual Prueba IdpruebaNavigation { get; set; } = null!;

    public virtual ProductoTerminado Producto { get; set; } = null!;
}
