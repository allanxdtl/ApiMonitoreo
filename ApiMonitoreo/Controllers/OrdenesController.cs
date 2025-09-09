using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiMonitoreo.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrdenController : ControllerBase
	{
		private readonly MonitoreoContext _context;

		public OrdenController(MonitoreoContext monitoreo)
		{
			_context = monitoreo;
			QuestPDF.Settings.License = LicenseType.Community;
		}

		[HttpPost("CrearOrden")]
		public async Task<IActionResult> CrearOrden(int idCliente, int idProducto, int cantidad)
		{
			if (idCliente <= 0 || idProducto <= 0 || cantidad <= 0)
				return BadRequest("Datos inválidos.");

			var producto = await _context.ProductoTerminados.FirstOrDefaultAsync(p => p.ProductoId == idProducto);
			if (producto == null) return NotFound("Producto no encontrado.");
			if (producto.ExistenciaActual < cantidad) return Conflict($"No hay suficiente existencia del producto '{producto.Nombre}'.");

			var nuevaOrden = new Orden
			{
				Idcliente = idCliente,
				Idproducto = idProducto,
				Cantidad = cantidad,
				FechaOrden = DateTime.Now
			};

			_context.Ordens.Add(nuevaOrden);
			producto.ExistenciaActual -= cantidad;
			await _context.SaveChangesAsync();

			var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == idCliente);

			try
			{
				var pdfBytes = GenerarPdfOrden(nuevaOrden, producto, cliente);
				return File(pdfBytes, "application/pdf", $"Orden_{nuevaOrden.Idorden}.pdf");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error al generar el PDF: {ex.Message}");
			}
		}

		private byte[] GenerarPdfOrden(Orden orden, ProductoTerminado producto, Cliente cliente)
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
							col.Item().Text("Detalles de la Orden").FontSize(16).Bold();
							col.Item().Text($"Orden ID: {orden.Idorden}").FontSize(12);
							col.Item().Text($"Fecha: {fechaImpresion}").FontSize(10).Italic();
							col.Item().Text($"Cliente: {cliente?.RazonSocial ?? "N/A"}").FontSize(12);
						});
					});

					// Contenido: tabla del producto
					page.Content().Table(table =>
					{
						table.ColumnsDefinition(columns =>
						{
							columns.RelativeColumn();
							columns.RelativeColumn();
							columns.RelativeColumn();
						});

						table.Header(header =>
						{
							header.Cell().Text("Producto").Bold();
							header.Cell().Text("Cantidad").Bold();
							header.Cell().Text("ID Producto").Bold();
						});

						// Fila del producto
						table.Cell().Text(producto.Nombre ?? "N/A");
						table.Cell().Text(orden.Cantidad.ToString());
						table.Cell().Text(orden.Idproducto.ToString());
					});

					// Pie de página
					page.Footer().AlignCenter().Text("Gracias por su orden.");
				});
			});

			return pdf.GeneratePdf(); // <-- clave: devolver byte[]
		}
	}
}
