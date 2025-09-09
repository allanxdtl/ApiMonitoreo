using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMonitoreo.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MateriaPrimaController : ControllerBase
	{
		private readonly MonitoreoContext _monitoreo;
		//Inyeccion de dependencia
		public MateriaPrimaController(MonitoreoContext context)
		{
			_monitoreo = context;
		}

		//Obtiene la lista de materias primas
		[HttpGet("List")]
		public async Task<IActionResult> Get()
		{
			var list = await _monitoreo.MateriaPrimas.Include(b => b.LoteMateriaPrimas).Select(m => new
			{
				m.MateriaPrimaId,
				m.Nombre,
				m.UnidadMedida,
				Existencia=m.LoteMateriaPrimas.Sum(l => l.CantidadDisponible)
			}).ToListAsync();

			return Ok(list);
		}

		//Obtiene una materia prima por su id
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var materiaPrima = await _monitoreo.MateriaPrimas.FindAsync(id);
			if (materiaPrima == null)
				return NotFound();


			return Ok( new { materiaPrima.MateriaPrimaId, materiaPrima.Nombre, materiaPrima.UnidadMedida });
		}

		[HttpGet]
		public async Task<IActionResult> Get(string text)
		{
			var results = await _monitoreo.MateriaPrimas
				.Include(b => b.LoteMateriaPrimas)
				.Where(m => m.Nombre.ToLower().Contains(text.ToLower()) || m.UnidadMedida.ToLower().Contains( text.ToLower()))
				.Select(m => new 
				{ 
					m.MateriaPrimaId,
					m.Nombre,
					m.UnidadMedida,
					Existencia= m.LoteMateriaPrimas.Sum(l => l.CantidadDisponible)
				})
				.ToListAsync();

			return Ok(results);
		}

		//Inserta una nueva materia prima
		[HttpPost("Insert")]
		public async Task<IActionResult> Insert(MateriaPrima materiaPrima)
		{
			_monitoreo.MateriaPrimas.Add(materiaPrima);
			await _monitoreo.SaveChangesAsync();
			return Ok(new { materiaPrima.MateriaPrimaId, materiaPrima.Nombre, materiaPrima.UnidadMedida });
		}

		//Actualiza una materia prima existente
		[HttpPut("Update")]
		public async Task<IActionResult> Update(int id, MateriaPrima materiaPrima)
		{
			if (id != materiaPrima.MateriaPrimaId)
				return BadRequest();
			_monitoreo.Entry(materiaPrima).State = EntityState.Modified;
			try
			{
				await _monitoreo.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_monitoreo.MateriaPrimas.Any(e => e.MateriaPrimaId == id))
					return NotFound();
				else
					throw;
			}
			return NoContent();
		}


		//Elimina una materia prima por su id
		[HttpDelete("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var materiaPrima = await _monitoreo.MateriaPrimas.FindAsync(id);
			if (materiaPrima == null)
				return NotFound();
			_monitoreo.MateriaPrimas.Remove(materiaPrima);
			await _monitoreo.SaveChangesAsync();
			return NoContent();
		}
	}
}
