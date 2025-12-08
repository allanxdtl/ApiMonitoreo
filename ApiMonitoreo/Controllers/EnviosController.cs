using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using System.Threading.Tasks;

namespace ApiMonitoreo.Controllers
{
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
				.Include(o => o.Produccions).ThenInclude(p=> p.SerieProductos)
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

		public IActionResult DescargarEtiquetas()
		{
			return Ok();
		}
	}
}
