using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string Usuario1 { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<HistorialPrueba> HistorialPruebas { get; set; } = new List<HistorialPrueba>();
}
