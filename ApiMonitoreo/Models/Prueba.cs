using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class Prueba
{
    public int Idprueba { get; set; }

    public string Descripcion { get; set; } = null!;

    public string? UnidadMedida { get; set; }

    public string? MetodoPrueba { get; set; }

    public virtual ICollection<HistorialPrueba> HistorialPruebas { get; set; } = new List<HistorialPrueba>();

    public virtual ICollection<PruebaProducto> PruebaProductos { get; set; } = new List<PruebaProducto>();
}
