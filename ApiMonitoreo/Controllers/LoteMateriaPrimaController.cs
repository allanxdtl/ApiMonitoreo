using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMonitoreo.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class LoteMateriaPrimaController : ControllerBase
	{
		private readonly MonitoreoContext _monitoreo;
		public LoteMateriaPrimaController(MonitoreoContext monitoreo)
		{
			_monitoreo = monitoreo;
		}

		[HttpPost("RegistrarLote")]
		public async Task<IActionResult> RegistrarLote([FromBody] LoteMateriaPrima lote)
		{
			if (lote == null || lote.MateriaPrimaId <= 0 || lote.CantidadInicial <= 0)
				return BadRequest("Datos inválidos");
			_monitoreo.LoteMateriaPrimas.Add(lote);
			await _monitoreo.SaveChangesAsync();
			return Ok("Lote de materia prima registrado correctamente");
		}

		[HttpGet("ObtenerExistencia/{id}")]
		public async Task<IActionResult> ObtenerExistencia(int id)
		{
			var existencia = await _monitoreo.LoteMateriaPrimas
				.Where(l => l.MateriaPrimaId == id)
				.SumAsync(l => l.CantidadDisponible);
			return Ok(new { MateriaPrimaId = id, ExistenciaActual = existencia });
		}
	}
}
