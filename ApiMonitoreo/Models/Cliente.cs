using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string? RazonSocial { get; set; }

    public string? Cp { get; set; }
}
