using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.QrCode;

namespace ApiMonitoreo.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class ProduccionController : ControllerBase
	{
		private readonly MonitoreoContext _context;
		public ProduccionController(MonitoreoContext monitoreo)
		{
			_context = monitoreo;
			QuestPDF.Settings.License = LicenseType.Community;
		}

		[HttpGet("GenerarCodigosBarraPDF/{produccionId}")]
		public async Task<IActionResult> GenerarCodigosBarraPDF(int produccionId)
		{
			try
			{
				// 1. Validar que la producción existe
				var produccion = await _context.Produccions.FindAsync(produccionId);
				if (produccion == null)
				{
					return NotFound("Producción no encontrada.");
				}

				// 2. Obtener los números de serie asociados a la producción
				var series = await _context.SerieProductos
					.Where(s => s.ProduccionId == produccionId)
					.Select(s => s.NumeroSerie)
					.ToListAsync();

				if (!series.Any())
				{
					return NotFound("No se encontraron números de serie para esta producción.");
				}

				// 3. Generar el PDF en memoria
				var pdfStream = new MemoryStream();
				Document.Create(container =>
				{
					container.Page(page =>
					{
						page.Size(PageSizes.A4);
						page.Margin(20);
						page.Content()
							.Column(column =>
							{
								column.Spacing(5);
								column.Item().Text("Códigos de Barra de Producción").FontSize(20).Bold().AlignCenter();
								column.Item().Text($"ID de Producción: {produccionId}").FontSize(12).AlignCenter();
								column.Item().PaddingTop(10);

								foreach (var serie in series)
								{
									// Generar el código de barras (usando una imagen de MemoryStream)
									var writer = new BarcodeWriterPixelData
									{
										Format = BarcodeFormat.CODE_128,
										Options = new QrCodeEncodingOptions
										{
											Height = 80,
											Width = 250
										}
									};
									var pixelData = writer.Write(serie);

									using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
									using var ms = new MemoryStream();
									var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
									System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
									bitmap.UnlockBits(bitmapData);
									bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

									column.Item().Column(innerColumn =>
									{
										// ❌ CAMBIO AQUÍ: Usar .FitWidth() en lugar de .FitHeight()
										innerColumn.Item().AlignCenter().Image(ms.ToArray()).FitWidth();
										innerColumn.Item().AlignCenter().Text(serie).FontSize(10);
										innerColumn.Item().PaddingVertical(10);
									});
								}
							});
					});
				}).GeneratePdf(pdfStream);

				pdfStream.Position = 0; // Resetear la posición del stream

				// 4. Devolver el PDF
				return File(pdfStream.ToArray(), "application/pdf", $"CodigosBarra_{produccionId}.pdf");
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error al generar el PDF: " + ex.Message);
			}
		}


		[HttpPost("CrearProduccion")]
		public async Task<IActionResult> CrearProduccion(int productoId, int cantidad, int orden)
		{
			// Validación de datos
			if (productoId <= 0 || cantidad <= 0)
				return BadRequest("Datos inválidos.");

			// Traer la receta del producto
			var receta = await _context.RecetaProductos
				.Where(r => r.ProductoId == productoId)
				.ToListAsync();

			if (!receta.Any())
				return NotFound("No existe receta para este producto.");

			// Verificar existencia de materia prima
			foreach (var item in receta)
			{
				decimal totalNecesario = item.CantidadNecesaria * cantidad;
				decimal totalDisponible = await _context.LoteMateriaPrimas
					.Where(l => l.MateriaPrimaId == item.MateriaPrimaId)
					.SumAsync(l => (decimal?)l.CantidadDisponible ?? 0);

				if (totalDisponible < totalNecesario)
				{
					return StatusCode(409, $"No hay suficiente existencia para la materia prima ID {item.MateriaPrimaId}");
				}
			}

			// Si hay suficiente, iniciamos la transacción
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				// 1. Insertar producción
				var produccion = new Produccion
				{
					ProductoId = productoId,
					CantidadProducida = cantidad,
					FechaProduccion = DateOnly.FromDateTime(DateTime.Now),
					OrdenId = orden
				};
				_context.Produccions.Add(produccion);
				await _context.SaveChangesAsync();

				// 2. Descontar materia prima y guardar consumo
				foreach (var item in receta)
				{
					decimal totalNecesario = item.CantidadNecesaria * cantidad;
					var lotes = await _context.LoteMateriaPrimas
						.Where(l => l.MateriaPrimaId == item.MateriaPrimaId && l.CantidadDisponible > 0)
						.OrderBy(l => l.FechaEntrada) // FIFO
						.ToListAsync();

					foreach (var lote in lotes)
					{
						if (totalNecesario <= 0) break;

						decimal usar = Math.Min(lote.CantidadDisponible, totalNecesario);

						// Insertar consumo
						var consumo = new ConsumoMateriaPrima
						{
							ProduccionId = produccion.ProduccionId,
							LoteId = lote.LoteId,
							MateriaPrimaId = item.MateriaPrimaId,
							CantidadUsada = usar
						};
						_context.ConsumoMateriaPrimas.Add(consumo);

						// Actualizar lote
						lote.CantidadDisponible -= usar;
						totalNecesario -= usar;
					}
				}

				// 3. Actualizar existencia del producto terminado
				var producto = await _context.ProductoTerminados.FirstOrDefaultAsync(p => p.ProductoId == productoId);
				if (producto != null)
				{
					producto.ExistenciaActual += cantidad;
				}

				await _context.SaveChangesAsync();

				// 4. Generar números de serie
				for (int i = 1; i <= cantidad; i++)
				{
					_context.SerieProductos.Add(new SerieProducto
					{
						ProduccionId = produccion.ProduccionId,
						NumeroSerie = $"PROD-{produccion.ProduccionId}-{DateTime.Now:yyyyMMdd}-{i:D3}"
					});
				}

				await _context.SaveChangesAsync();

				var ordenDB = await _context.Ordens.FirstOrDefaultAsync(o => o.Idorden == orden);
				ordenDB.Estatus = "Listo para pruebas";

				await _context.SaveChangesAsync();

				// 5. Confirmar transacción
				await transaction.CommitAsync();

				return Ok(produccion.ProduccionId);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return StatusCode(500, "Error en la producción: " + ex.Message);
			}
		}
	}
}
