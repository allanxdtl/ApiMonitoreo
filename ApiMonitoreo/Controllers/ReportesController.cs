using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ApiMonitoreo.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class ReportesController : ControllerBase
	{
		private readonly MonitoreoContext _context;

		public ReportesController(MonitoreoContext ctx)
		{
			QuestPDF.Settings.License = LicenseType.Community;
			_context = ctx;
		}

		// ================================
		//  REPORTE: Venta - Scrap
		// ================================
		[HttpGet("venta/pdf")]
		public async Task<IActionResult> VentaScrapPdf(DateTime fecha1, DateTime fecha2)
		{
			var ordenes = await _context.Ordens
				.Include(o => o.Produccions)
					.ThenInclude(p => p.SerieProductos)
				.Where(o => o.FechaOrden >= fecha1 && o.FechaOrden <= fecha2)
				.ToListAsync();

			decimal venta = 0;
			decimal scrap = 0;

			foreach (var o in ordenes)
			{
				int pasa = o.Produccions.SelectMany(p => p.SerieProductos)
										.Count(s => s.EstatusCalidad == "PASA");

				int scrapCount = o.Produccions.SelectMany(p => p.SerieProductos)
										.Count(s => s.EstatusCalidad != "SCRAP");

				if (o.Cantidad > 0)
				{
					decimal precioUnitario = (decimal)o.Precio / (decimal)o.Cantidad;
					venta += pasa * precioUnitario;
					scrap += scrapCount * precioUnitario;
				}
			}

			var utilidad = venta - scrap;

			var pdf = GenerarPdfBonito("Reporte Venta - Scrap", fecha1, fecha2, venta, scrap, utilidad);

			return File(pdf, "application/pdf", "Reporte_VentaScrap.pdf");
		}

		// ================================
		//  REPORTE: SOLO SCRAP
		// ================================
		[HttpGet("scrap/pdf")]
		public async Task<IActionResult> ScrapPdf(DateTime fecha1, DateTime fecha2)
		{
			var ordenes = await _context.Ordens
				.Include(o => o.Produccions)
					.ThenInclude(p => p.SerieProductos)
				.Where(o => o.FechaOrden >= fecha1 && o.FechaOrden <= fecha2)
				.ToListAsync();

			decimal scrap = 0;

			foreach (var o in ordenes)
			{
				int scrapCount = o.Produccions
					.SelectMany(p => p.SerieProductos)
					.Count(s => s.EstatusCalidad != "PASA");

				if (o.Cantidad > 0)
				{
					decimal precioUnitario = (decimal)o.Precio / (decimal)o.Cantidad;
					scrap += scrapCount * precioUnitario;
				}
			}

			var pdf = GenerarPdfBonito("Reporte Scrap", fecha1, fecha2, 0, scrap, -scrap);

			return File(pdf, "application/pdf", "Reporte_Scrap.pdf");
		}


		// ================================
		//  REPORTE: VENTAS TOTALES
		// ================================
		[HttpGet("total/pdf")]
		public async Task<IActionResult> TotalVentasPdf(DateTime fecha1, DateTime fecha2)
		{
			var ordenes = await _context.Ordens
				.Include(o => o.Produccions)
					.ThenInclude(p => p.SerieProductos)
				.Where(o => o.FechaOrden >= fecha1 && o.FechaOrden <= fecha2)
				.ToListAsync();

			decimal venta = 0;

			foreach (var o in ordenes)
			{
				int pasa = o.Produccions
					.SelectMany(p => p.SerieProductos)
					.Count(s => s.EstatusCalidad == "PASA");

				if (o.Cantidad > 0)
				{
					decimal precioUnitario = (decimal)o.Precio / (decimal)o.Cantidad;
					venta += pasa * precioUnitario;
				}
			}

			var pdf = GenerarPdfBonito("Reporte de Ventas Totales", fecha1, fecha2, venta, 0, venta);

			return File(pdf, "application/pdf", "Reporte_VentasTotales.pdf");
		}

		private byte[] GenerarPdfBonito(string titulo, DateTime fecha1, DateTime fecha2,
	decimal venta, decimal scrap, decimal utilidad)
		{
			return Document.Create(document =>
			{
				document.Page(page =>
				{
					page.Margin(30);
					page.Size(PageSizes.A4);

					page.Content().Column(col =>
					{
						// --------------------------
						// Título principal
						// --------------------------
						col.Item().Container().Background(Colors.Grey.Lighten3).Padding(20).CornerRadius(10).Column(header =>
						{
							header.Item().Text(titulo).FontSize(22).Bold().FontColor(Colors.Blue.Medium);
							header.Item().Text($"Desde {fecha1:yyyy-MM-dd} hasta {fecha2:yyyy-MM-dd}")
								  .FontSize(12)
								  .FontColor(Colors.Grey.Darken2);
						});

						col.Item().PaddingTop(15);

						// Divider
						col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

						col.Item().PaddingTop(20);

						// --------------------------
						// Tarjetas de KPIs
						// --------------------------
						col.Item().Row(row =>
						{
							row.Spacing(15);

							// Venta
							row.RelativeItem().Container().Padding(15).Border(1).BorderColor(Colors.Green.Darken2).Background(Colors.Green.Lighten5).CornerRadius(8).Column(box =>
							{
								box.Item().Text("VENTA").FontSize(14).Bold().FontColor(Colors.Green.Darken3);
								box.Item().Text($"${venta:N2}").FontSize(20).Bold();
							});

							// Scrap
							row.RelativeItem().Container().Padding(15).Border(1).BorderColor(Colors.Red.Darken2).Background(Colors.Red.Lighten5).CornerRadius(8).Column(box =>
							{
								box.Item().Text("SCRAP").FontSize(14).Bold().FontColor(Colors.Red.Darken3);
								box.Item().Text($"${scrap:N2}").FontSize(20).Bold();
							});

							// Utilidad Neta
							row.RelativeItem().Container().Padding(15).Border(1).BorderColor(Colors.Blue.Darken2).Background(Colors.Blue.Lighten5).CornerRadius(8).Column(box =>
							{
								box.Item().Text("UTILIDAD NETA").FontSize(14).Bold().FontColor(Colors.Blue.Darken3);
								box.Item().Text($"${utilidad:N2}").FontSize(20).Bold();
							});
						});

						col.Item().PaddingTop(25);

						// --------------------------
						// Sección final opcional
						// --------------------------
						col.Item().Container().Padding(10).Background(Colors.Grey.Lighten4).CornerRadius(6).Column(f =>
						{
							f.Item().Text("Reporte generado automáticamente por el sistema de monitoreo.")
									.FontSize(10)
									.FontColor(Colors.Grey.Darken2);

							f.Item().Text($"Generado el {DateTime.Now:yyyy-MM-dd HH:mm}")
									.FontColor(Colors.Grey.Darken2)
									.FontSize(10);
						});
					});
				});
			}).GeneratePdf();
		}

	}
}
