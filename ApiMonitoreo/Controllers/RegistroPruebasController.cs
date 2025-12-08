using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMonitoreo.Controllers
{
	public class PruebaInyectDTO
	{
		public string NoSerie { get; set; }
		public int IdPrueba { get; set; }
		public decimal ValorMedido { get; set; }
	}

	[Route("api/[controller]")]
	[ApiController]
	public class RegistroPruebasController : ControllerBase
	{
		private readonly MonitoreoContext _context;

		public RegistroPruebasController(MonitoreoContext context)
		{
			_context = context;
		}

		[HttpPost("RealizarPrueba")]
		public async Task<IActionResult> Post([FromBody] PruebaInyectDTO valores)
		{
			var serie = await _context.SerieProductos.Include(s => s.Produccion)
				.FirstOrDefaultAsync(s => s.NumeroSerie == valores.NoSerie);

			var pruebaExistente = await _context.HistorialPruebas
				.FirstOrDefaultAsync(p =>
					p.Idprueba == valores.IdPrueba &&
					p.SerieId == serie.SerieId
				);


			if (serie == null || pruebaExistente != null)
				return NotFound("El numero de serie proporcionado no se le puede realizar la prueba que solicita");

			var reg = new HistorialPrueba();
			reg.Idprueba = valores.IdPrueba;
			reg.SerieId = serie.SerieId;
			reg.ValorMedido = valores.ValorMedido;

			await _context.HistorialPruebas.AddAsync(reg);
			await _context.SaveChangesAsync();

			var NumeroDePruebas = _context.HistorialPruebas.Where(hp => hp.SerieId == reg.SerieId).Count();

			if (NumeroDePruebas == 4)
			{
				// Obtener las 4 pruebas realizadas
				var pruebasRealizadas = await _context.HistorialPruebas
					.Where(hp => hp.SerieId == reg.SerieId)
					.ToListAsync();

				// Obtener los valores esperados y tolerancias de esas pruebas
				var catalogoPruebas = await _context.PruebaProductos
					.Where(pp => pruebasRealizadas.Select(pr => pr.Idprueba).Contains(pp.Idprueba))
					.ToListAsync();

				bool todasPasan = true;

				foreach (var realizada in pruebasRealizadas)
				{
					var catalogo = catalogoPruebas.First(pp => pp.Idprueba == realizada.Idprueba);

					var diferencia = Math.Abs(realizada.ValorMedido - catalogo.ValorEsperado);

					if (diferencia > catalogo.Tolerancia)
					{
						todasPasan = false;
						break;
					}
				}

				// Actualizar estatus dependiendo del resultado
				SerieProducto? serieActual =
					await _context.SerieProductos.FirstOrDefaultAsync(sp => sp.SerieId == reg.SerieId);

				serieActual.EstatusCalidad = todasPasan ? "PASA" : "NO PASA";

				await _context.SaveChangesAsync();
			}

			var ordenId = serie.Produccion.OrdenId;

			// Obtener todas las series de la producción
			var seriesDeProduccion = await _context.SerieProductos
				.Where(sp => sp.Produccion.OrdenId == ordenId)
				.ToListAsync();

			bool todasSeriesCompletas = true;

			foreach (var s in seriesDeProduccion)
			{
				int count = await _context.HistorialPruebas
					.CountAsync(hp => hp.SerieId == s.SerieId);

				if (count < 4)
				{
					todasSeriesCompletas = false;
					break;
				}
			}

			if (todasSeriesCompletas)
			{
				var produccion = await _context.Ordens
					.FirstOrDefaultAsync(p => p.Idorden == ordenId);

				if (produccion != null)
				{
					produccion.Estatus = "Listo para Envio";
					await _context.SaveChangesAsync();
				}
			}


			return Ok(new { message = "Prueba registrada correctamente" });
		}

		[HttpGet("Resultados/{noSerie}")]
		public async Task<IActionResult> GetProduct(string noSerie)
		{
			if (noSerie.Length <= 4)
			{
				var query = await _context.SerieProductos
									.Where(sp => sp.Produccion.OrdenId == int.Parse(noSerie))   // Filtrar por la orden dada
									.Select(sp => new
									{
										NumeroSerie = sp.NumeroSerie,

										Prueba1 = sp.HistorialPruebas
											.Where(h => h.Idprueba == 1)
											.Select(h => (decimal?)h.ValorMedido)
											.FirstOrDefault(),

										Prueba2 = sp.HistorialPruebas
											.Where(h => h.Idprueba == 2)
											.Select(h => (decimal?)h.ValorMedido)
											.FirstOrDefault(),

										Prueba3 = sp.HistorialPruebas
											.Where(h => h.Idprueba == 3)
											.Select(h => (decimal?)h.ValorMedido)
											.FirstOrDefault(),

										Prueba4 = sp.HistorialPruebas
											.Where(h => h.Idprueba == 4)
											.Select(h => (decimal?)h.ValorMedido)
											.FirstOrDefault(),

										Resultado =
											sp.EstatusCalidad
									})
						.ToListAsync();
				return Ok(query);
			}

			var result = await _context.HistorialPruebas
				.Join(_context.SerieProductos, hp => hp.SerieId, sp => sp.SerieId, (hp, sp) => new { hp, sp })
				.Where(s => s.sp.NumeroSerie == noSerie)
				.Join(_context.Pruebas, x => x.hp.Idprueba, p => p.Idprueba, (x, p) => new { x.hp, x.sp, p })
				.Join(_context.PruebaProductos, x => x.p.Idprueba, pp => pp.Idprueba, (x, pp) => new
				{
					Descripcion = x.p.Descripcion,
					ValorMedido = x.hp.ValorMedido,
					ValorEsperado = pp.ValorEsperado,
					Tolerancia = pp.Tolerancia,
					Resultado =
						Math.Abs(x.hp.ValorMedido - pp.ValorEsperado) <= pp.Tolerancia
							? "PASA"
							: "NO PASA"
				})
				.ToListAsync();

			return Ok(result);
		}
	}
}
