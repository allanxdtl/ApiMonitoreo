using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiMonitoreo.Models;

public partial class RecetaProducto
{
    public int RecetaId { get; set; }

    public int ProductoId { get; set; }

    public int MateriaPrimaId { get; set; }

    public decimal CantidadNecesaria { get; set; }

	[JsonIgnore]
	[ValidateNever]
	public virtual MateriaPrima MateriaPrima { get; set; } = null!;

	[JsonIgnore]
	[ValidateNever]
	public virtual ProductoTerminado Producto { get; set; } = null!;
}
