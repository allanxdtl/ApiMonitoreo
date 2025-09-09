using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ApiMonitoreo.Controllers
{
	public class BOMDetalle
	{
		public int IdMateriaPrima { get; set; }
		public string NombreMateriaPrima { get; set; }
		public decimal CantidadNecesaria { get; set; }
		public decimal ExistenciaActualMP { get; set; }
	}


	[ApiController]
	[Route("api/[controller]")]
	public class BOMController : ControllerBase
	{
		private readonly MonitoreoContext _context;

		public BOMController(MonitoreoContext context)
		{
			_context = context;
			QuestPDF.Settings.License = LicenseType.Community;
		}

		// Insertar una relación producto-materia prima (BOM)
		[HttpPost("Insert")]
		public async Task<IActionResult> Insert([FromBody] RecetaProducto bom)
		{
			if (bom == null || bom.ProductoId <= 0 || bom.MateriaPrimaId <= 0 || bom.CantidadNecesaria <= 0)
				return BadRequest("Datos inválidos");

			_context.RecetaProductos.Add(bom);
			await _context.SaveChangesAsync();
			return Ok("BOM agregado correctamente");
		}

		// Consultar el BOM de un producto
		[HttpGet("GetByProducto/{productoId}")]
		public async Task<IActionResult> GetByProducto(int productoId)
		{
			// Obtener el producto terminado
			var producto = _context.ProductoTerminados.FirstOrDefault(p => p.ProductoId == productoId);
			if (producto == null) return NotFound("Producto no encontrado");

			// Obtener detalles del BOM
			var detalles = await (from b in _context.RecetaProductos
							join mp in _context.MateriaPrimas on b.MateriaPrimaId equals mp.MateriaPrimaId
							where b.ProductoId == productoId
							select new BOMDetalle
							{
								IdMateriaPrima = mp.MateriaPrimaId,
								NombreMateriaPrima = mp.Nombre,
								CantidadNecesaria = b.CantidadNecesaria,
								ExistenciaActualMP = _context.LoteMateriaPrimas
									.Where(l => l.MateriaPrimaId == mp.MateriaPrimaId)
									.Sum(l => (decimal?)l.CantidadDisponible) ?? 0
							}).ToListAsync();

			// Existencia actual del producto terminado
			decimal existenciaProducto = _context.ProductoTerminados
				.Where(p => p.ProductoId == productoId)
				.Select(p => p.ExistenciaActual)
				.FirstOrDefault();

			// Generar PDF
			var pdfBytes = GenerarPdfBOM(producto.ProductoId, producto.Nombre, detalles, existenciaProducto);

			// Retornar archivo para descarga
			return File(pdfBytes, "application/pdf", $"BOM_{producto.ProductoId}.pdf");
		}

		private byte[] GenerarPdfBOM(int productoId, string nombreProducto, List<BOMDetalle> detalles, decimal existenciaProducto)
		{
			var fechaImpresion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

			var pdf = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(30);

					// Encabezado
					page.Header().Row(row =>
					{
						row.RelativeItem().Column(col =>
						{
							col.Item().Text($"Reporte BOM - Producto Terminado").FontSize(16).Bold();
							col.Item().Text($"Producto ID: {productoId} - {nombreProducto}").FontSize(12);
							col.Item().Text($"Fecha: {fechaImpresion}").FontSize(10).Italic();
						});
					});

					// Contenido: Tabla con detalles
					page.Content().Table(table =>
					{
						table.ColumnsDefinition(columns =>
						{
							columns.ConstantColumn(100);  // ID MP
							columns.RelativeColumn();     // Nombre MP
							columns.ConstantColumn(100);  // Cantidad Necesaria
							columns.ConstantColumn(100);  // Existencia MP
						});

						// Encabezado de la tabla
						table.Header(header =>
						{
							header.Cell().Text("ID Materia Prima").Bold();
							header.Cell().Text("Nombre Materia Prima").Bold();
							header.Cell().Text("Cantidad Necesaria").Bold();
							header.Cell().Text("Existencia Actual MP").Bold();
						});

						// Filas con datos
						foreach (var d in detalles)
						{
							table.Cell().Text(d.IdMateriaPrima.ToString());
							table.Cell().Text(d.NombreMateriaPrima);
							table.Cell().Text(d.CantidadNecesaria.ToString());
							table.Cell().Text(d.ExistenciaActualMP.ToString());
						}
					});

					// Pie de página con existencia del producto
					page.Footer().AlignLeft().Text($"Existencia actual del producto terminado: {existenciaProducto}");
				});
			});

			return pdf.GeneratePdf();
		}
	}
}
