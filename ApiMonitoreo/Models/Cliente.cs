using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string? RazonSocial { get; set; }

    public string? Cp { get; set; }

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
