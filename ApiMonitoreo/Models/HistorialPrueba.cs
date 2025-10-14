using System;
using System.Collections.Generic;

namespace ApiMonitoreo.Models;

public partial class HistorialPrueba
{
    public int Idhistorial { get; set; }

    public int SerieId { get; set; }

    public int Idprueba { get; set; }

    public decimal ValorMedido { get; set; }

    public DateTime? FechaPrueba { get; set; }

    public int? UsuarioId { get; set; }

    public virtual Prueba IdpruebaNavigation { get; set; } = null!;

    public virtual SerieProducto Serie { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
