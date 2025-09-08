using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMonitoreo.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BOMController : ControllerBase
	{
		private readonly MonitoreoContext _context;
		public BOMController(MonitoreoContext context)
		{
			_context = context;
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
			var bom = await _context.RecetaProductos
				.Include(b => b.Producto)
				.Include(b => b.MateriaPrima)
				.Where(b => b.ProductoId == productoId)
				.Select(b => new
				{
					Producto = b.Producto.Nombre,
					MateriaPrima = b.MateriaPrima.Nombre,
					b.CantidadNecesaria,
					b.MateriaPrima.UnidadMedida
				})
				.ToListAsync();

			if (bom == null || bom.Count == 0)
				return NotFound("No hay datos para este producto");

			return Ok(bom);
		}
	}
}
