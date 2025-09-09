using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMonitoreo.Controllers
{
	public class ProduccionController : ControllerBase
	{
		private readonly MonitoreoContext _context;
		public ProduccionController(MonitoreoContext monitoreo)
		{
			_context = monitoreo;
		}



		[HttpPost("CrearProduccion")]
		public async Task<IActionResult> CrearProduccion(int productoId, int cantidad)
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
					FechaProduccion = DateOnly.FromDateTime(DateTime.Now)
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
						NumeroSerie = $"PROD-{productoId}-{DateTime.Now:yyyyMMdd}-{i:D3}"
					});
				}

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
