using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Threading.Tasks;

namespace ApiMonitoreo.Controllers
{
	class Cajas
	{
		public SerieProducto producto { get; set; }
		public int index { get; set; }
	}

	[Route("api/[controller]")]
	[ApiController]
	public class EnviosController : ControllerBase
	{
		public EnviosController(MonitoreoContext context)
		{
			QuestPDF.Settings.License = LicenseType.Community;
			_context = context;
		}

		private readonly MonitoreoContext _context;

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var ordenes = await _context.Ordens.Where(o => o.Estatus == "Listo para Envio").ToListAsync();

			return Ok(ordenes);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetOrden(string id)
		{
			var ordenes = await _context.Ordens
				.Include(o => o.IdclienteNavigation)
				.Include(o => o.IdproductoNavigation)
				.Include(o => o.Produccions).ThenInclude(p => p.SerieProductos)
				.Select(o => new
				{
					idorden = o.Idorden,
					idcliente = o.IdclienteNavigation.RazonSocial,
					precio = o.Precio,
					estatus = o.Estatus,
					idproducto = o.IdproductoNavigation.Nombre,
					cantidadPedida = o.Cantidad,
					cantidadProductosPasados = o.Produccions
						.SelectMany(p => p.SerieProductos)
						.Count(s => s.EstatusCalidad == "PASA")

				}).FirstOrDefaultAsync(r => r.idorden == int.Parse(id));

			return Ok(ordenes);
		}

		[HttpGet("DescargarEtiquetasDeEmbalaje")]
		public async Task<IActionResult> DescargarEtiquetas(string ordenid)
		{
			int id = int.Parse(ordenid);

			var orden = await _context.Ordens
				.Include(o => o.IdclienteNavigation)
				.Include(o => o.Produccions).ThenInclude(p => p.SerieProductos)
				.FirstOrDefaultAsync(o => o.Idorden == id);

			if (orden == null)
				return NotFound("Orden no encontrada");

			// Obtener solo los productos que PASARON
			var productosPasados = orden.Produccions
				.SelectMany(p => p.SerieProductos)
				.Where(s => s.EstatusCalidad == "PASA")
				.ToList();

			if (productosPasados.Count == 0)
				return BadRequest("No hay productos en estado PASA");

			// Agrupar en cajas de 8
			var cajas = productosPasados
				.Select((producto, index) => new Cajas { producto = producto, index = index })
				.GroupBy(x => x.index / 8)
				.Select(g => g.ToList())
				.ToList();

			byte[] pdf = GenerateEtiquetasPdf(orden, cajas);

			return File(pdf, "application/pdf", $"Etiquetas_Orden_{ordenid}.pdf");
		}

		private byte[] GenerateQr(string text)
		{
			using var qr = new QRCoder.QRCodeGenerator();
			var data = qr.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.Q);
			var png = new QRCoder.PngByteQRCode(data);

			return png.GetGraphic(20);
		}


		private byte[] GenerateEtiquetasPdf(Orden orden, List<List<Cajas>> cajas)
		{
			var cliente = orden.IdclienteNavigation;

			return QuestPDF.Fluent.Document.Create(document =>
			{
				document.Page(page =>
				{
					page.Margin(20);
					page.Size(PageSizes.A4);

					page.Content().Column(col =>
					{
						int totalCajas = cajas.Count;

						for (int i = 0; i < totalCajas; i++)
						{
							string tracking = $"TRK-{orden.Idorden}-{i + 1}-{DateTime.Now:yyyyMMddHHmmss}";

							col.Item().Border(2).Padding(20).Column(box =>
							{
								box.Item().Text($"ETIQUETA DE ENVÍO").Bold().FontSize(20).AlignCenter();
								box.Item().PaddingTop(10);

								box.Item().Column(info =>
								{
									info.Item().Text($"Cliente: {cliente.RazonSocial}").FontSize(14).Bold();
									info.Item().Text($"Código Postal: {cliente.Cp}").FontSize(13);
									info.Item().Text($"Orden: {orden.Idorden}").FontSize(13);
									info.Item().Text($"Caja {i + 1} de {totalCajas}").FontSize(13).Bold();
								});

								box.Item().PaddingTop(15);

								box.Item().BorderTop(1).PaddingTop(10);

								box.Item().Column(track =>
								{
									track.Item().Text("Tracking Number:").Bold().FontSize(14);
									track.Item().Text(tracking).FontSize(16).Bold();
								});

								// Código QR del tracking
								box.Item().PaddingTop(10);
								box.Item().Image(GenerateQr(tracking), ImageScaling.FitWidth);
							});

							// Separador entre etiquetas
							col.Item().PageBreak();
						}
					});
				});
			}).GeneratePdf();
		}


	}
}
