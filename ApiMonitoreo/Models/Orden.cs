using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class Orden
{
    public int Idorden { get; set; }

    public int Idcliente { get; set; }

    public int Idproducto { get; set; }

    public int Cantidad { get; set; }

    public DateTime FechaOrden { get; set; }

    public string Estatus { get; set; } = null!;

    public virtual Cliente IdclienteNavigation { get; set; } = null!;

    public virtual ProductoTerminado IdproductoNavigation { get; set; } = null!;

    public virtual ICollection<Produccion> Produccions { get; set; } = new List<Produccion>();
}
